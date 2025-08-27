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

GetAcmeTxtRecordContents () {
	dig +noall +answer TXT "$CREATE_DOMAIN" | cut -d '"' -f 2
}

DoCheck () {
	sleep 60
	CheckAcmeTxtRecordExists || (echo "Record not yet in DNS" && DoCheck)
	CURRENT=$(GetAcmeTxtRecordContents)
	if [ "$CURRENT" != "$CERTBOT_VALIDATION" ]; then
		echo "$CURRENT != $CERTBOT_VALIDATION"
		DoCheck
	fi
}

DoCreate () {
	CheckAcmeTxtRecordExists && Panic "Stale ACME record for host found in DNS"
	CheckAcmeTxtRecordExists || (echo "$HERACLES_STDIN" | heracles add    || Panic "Hydra DNS update failed")
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
