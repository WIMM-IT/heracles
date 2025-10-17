#!/bin/bash
#
# https://eff-certbot.readthedocs.io/en/stable/using.html#pre-and-post-validation-hooks
#
# Passed environment variables:
#
# $CERTBOT_DOMAIN The domain being authenticated
# $CERTBOT_VALIDATION The validation string
# $CERTBOT_TOKEN Resource name part of the HTTP-01 challenge (HTTP-01 only)
# $CERTBOT_REMAINING_CHALLENGES Number of challenges remaining after the current challenge
# $CERTBOT_ALL_DOMAINS A comma-separated list of all domains challenged for the current certificate
#
# Usage:
#
# Ensure that /etc/heracles.conf has been created
#
# certbot certonly --manual --preferred-challenges=dns \
#                  --manual-auth-hook /path/to/CertBotAuth.sh \
#                  --manual-cleanup-hook /path/to/CertBotCleanup.sh
#
# To Do:
#
# - Handle multi-domain certificate requests

RESOLVERS=("1.1.1.1" "8.8.8.8" "9.9.9.9" "208.67.222.222")

CREATE_DOMAIN="_acme-challenge.$CERTBOT_DOMAIN"
HERACLES_STDIN="[{ \"comment\": \"WinAcme\", \"content\": \"$CERTBOT_VALIDATION\", \"hostname\": \"$CREATE_DOMAIN.\", \"type\": \"TXT\" }]"

Panic () {
	echo "$1"
	exit 1
}

CheckBin () {
	which "$1" &>/dev/null || Panic "Cannot find executable $1"
}

CheckAcmeTxtRecordExists () {
	RES=$(dig +noall +answer TXT "$CREATE_DOMAIN")
	if [[ -z $RES ]]; then
		return 1
	else
		return 0
	fi
}

CheckAcmeTxtRecordPropagated () {
	local resolver_up=0
	for ((idx=${#RESOLVERS[@]}-1; idx>=0; idx--)); do
		resolver="${RESOLVERS[$idx]}"
		if ! dig +tries=1 +time=2 @"${resolver}" . NS >/dev/null 2>&1; then
			continue
		fi
		resolver_up=1
        result=$(dig +short TXT "${CREATE_DOMAIN}" @"${resolver}" | tr -d '"')
		if [[ "$result" != *"$CERTBOT_VALIDATION"* ]]; then
            return 1  # fail
        else
            # Don't need to ping this resolver again.
			unset 'RESOLVERS[idx]'
        fi
    done
	if [[ $resolver_up -eq 0 ]]; then
        # No resolvers reachable
		return 1
	fi
	return 0  # success
}

GetAcmeTxtRecordContents () {
	dig +noall +answer TXT "$CREATE_DOMAIN" | cut -d '"' -f 2
}

DoCheck () {
	sleep 60
	CheckAcmeTxtRecordPropagated || (echo "Record not yet in DNS" && DoCheck)
	CURRENT=$(GetAcmeTxtRecordContents)
	if [ "$CURRENT" != "$CERTBOT_VALIDATION" ]; then
		echo "$CURRENT != $CERTBOT_VALIDATION"
		DoCheck
	fi
}

DoCreate () {
	CheckAcmeTxtRecordPropagated && Panic "Stale ACME record for host found in DNS"
	CheckAcmeTxtRecordPropagated || (echo "$HERACLES_STDIN" | heracles add    || Panic "Hydra DNS update failed")
	DoCheck
	sleep 60
}

CheckBin dig
CheckBin nslookup
CheckBin heracles

source /etc/heracles.conf || Panic "Cannot open /etc/heracles.conf"
if [ -z "$HYDRA_URI" ]; then
  Panic "HYDRA_URI is not set"
fi
if [ -z "$HYDRA_TOKEN" ]; then
  Panic "HYDRA_TOKEN is not set"
fi

nslookup "$CERTBOT_DOMAIN" &> /dev/null || Panic "$CERTBOT_DOMAIN is not a valid DNS entry"
DoCreate
