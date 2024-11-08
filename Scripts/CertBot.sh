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
# certbot certonly --manual --preferred-challenges=dns --manual-auth-hook /path/to/CertBot.sh

CREATE_DOMAIN="_acme-challenge.$CERTBOT_DOMAIN"
HERACLES_STDIN="[{ \"comment\": \"WinAcme\", \"content\": \"$CERTBOT_VALIDATION\", \"hostname\": \"$CREATE_DOMAIN.\", \"type\": \"TXT\" }]"

Panic () {
	echo $1
	exit 1
}

CheckBin () {
	which $1 &>/dev/null || Panic "Cannot find executable $1"
}

CheckAcmeTxtRecordExists () {
	RES=`dig +noall +answer TXT $CREATE_DOMAIN`
	if [[ -z $RES ]]; then
		return 1
	else
		return 0
	fi
}

GetAcmeTxtRecordContents () {
	dig +noall +answer TXT $CREATE_DOMAIN | cut -d '"' -f 2
}

DoCheck () {
	sleep 60
	CheckAcmeTxtRecordExists || (echo "Record not yet in DNS" && DoCheck)
	CURRENT=$(GetAcmeTxtRecordContents)
	if [ $CURRENT != $CERTBOT_VALIDATION ]; then
		echo "$CURRENT != $CERTBOT_VALIDATION"
		DoCheck
	fi
}

DoCreate () {
	CheckAcmeTxtRecordExists && (echo $HERACLES_STDIN | heracles update || Panic "Hydra DNS update failed")
	CheckAcmeTxtRecordExists || (echo $HERACLES_STDIN | heracles add    || Panic "Hydra DNS update failed")
	DoCheck
}

CheckBin dig
CheckBin nslookup
CheckBin heracles

nslookup $CERTBOT_DOMAIN &> /dev/null || Panic "$CERTBOT_DOMAIN is not a valid DNS entry"
DoCreate
