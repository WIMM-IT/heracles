#!/bin/bash

CREATE_DOMAIN="_acme-challenge.$CERTBOT_DOMAIN"

Panic () {
	echo "$1"
	exit 1
}

CheckBin () {
	which "$1" &>/dev/null || Panic "Cannot find executable $1"
}

CheckAcmeTxtRecordExists () {
	RES=$(dig +noall 5+answer TXT "$CREATE_DOMAIN")
	if [[ -z $RES ]]; then
		return 1
	else
		return 0
	fi
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
CheckAcmeTxtRecordExists || Panic "$CREATE_DOMAIN is not a valid DNS entry"
heracles search "$CREATE_DOMAIN" > hydra_delete.txt
RECORD_COUNT=$(cat hydra_delete.txt | tr -cd '{' | wc -c)
if [ "$RECORD_COUNT" -ne 1 ]; then
	Panic "Attempted to delete multiple Hydra records"
fi
cat hydra_delete.txt | heracles delete
