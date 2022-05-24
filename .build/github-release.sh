#!/bin/bash

ReleaseName=$1
Tag=$2
GitHubToken=$3
GitHubRepo=$4

echo "Creating GitHub Release $ReleaseName..."
echo "Calling - https://api.github.com/repos/$GitHubRepo/releases"

CreateReleaseResponse=$(curl --request POST \
    --url "https://api.github.com/repos/$GitHubRepo/releases" \
    --header "authorization: Bearer $GitHubToken" \
    --header "content-type: application/json" \
    --data '{
                "tag_name": "'"$Tag"'",
                "target_commitish": "master",
                "name": "'"$ReleaseName"'",
                "body": "",
                "draft": false,
                "prerelease": false }')

echo "$CreateReleaseResponse" # Print the message
echo "GitHub Release created"