#!/bin/bash

NEXT_VERSION="$1"

# Read Version parts
IFS='.'  
read -a VERSION_PARTS <<<"$NEXT_VERSION"
  
MAJOR="${VERSION_PARTS[0]}"
MINOR="${VERSION_PARTS[1]}"
PATCH=0

# Get last tag
LATEST_TAG=$(git describe --tags --abbrev=0 2>/dev/null)
HEAD_HASH=$(git rev-parse --verify HEAD)
TAG_HASH=$(git log -1 --format=format:"%H" $LATEST_TAG | tail -n1)

if [[ -z "$LATEST_TAG" ]]; then
    echo "$NEXT_VERSION.0"
    exit
fi

if [[ "$HEAD_HASH" == "$TAG_HASH" ]]; then
    echo $LATEST_TAG
    exit
fi

# Read Version parts
IFS='.'  
read -a VERSION_PARTS <<<"$LATEST_TAG"
  
LATEST_MAJOR="${VERSION_PARTS[0]}"
LATEST_MINOR="${VERSION_PARTS[1]}"
LATEST_PATCH="${VERSION_PARTS[2]}"

if [[ "$MAJOR" > "$LATEST_MAJOR" ]]; then
    echo "${MAJOR}.${MINOR}.${PATCH}"
    exit
elif [[ "$MINOR" > "$LATEST_MINOR" ]]; then
    echo "${MAJOR}.${MINOR}.${PATCH}"
    exit
else
    ((LATEST_PATCH++))
    echo "${MAJOR}.${MINOR}.${LATEST_PATCH}"
    exit
fi