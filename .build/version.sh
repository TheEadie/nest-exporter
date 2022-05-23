#!/bin/bash

NEXT_VERSION="$1"
MAJOR=0
MINOR=0
PATCH=0

# Read Version parts
IFS='.'  
read -a VERSION_PARTS <<<"$NEXT_VERSION" #reading str as an array as tokens separated by IFS  
  
MAJOR="${VERSION_PARTS[0]}"
MINOR="${VERSION_PARTS[1]}"


echo "${MAJOR}.${MINOR}.${PATCH}"