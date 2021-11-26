USERNAME=$1
PASSWORD=$2

# Build
docker build -t theeadie/nest-exporter:latest .

# Publish
echo $PASSWORD | docker login -u $USERNAME --password-stdin
docker push theeadie/nest-exporter:latest
