USERNAME=$1
PASSWORD=$2

# Publish
echo $PASSWORD | docker login -u $USERNAME --password-stdin
docker buildx build -t theeadie/nest-exporter:latest . --platform linux/amd64,linux/arm64  --push
