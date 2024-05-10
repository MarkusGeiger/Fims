#! /bin/zsh
echo "Using ssh to update this repository on vm and run deploy script."
ssh root@192.168.2.10 'cd src/Fims;pwd;git pull;cd Source;pwd;docker compose build;docker compose down && docker compose up --detach'
echo "Target updated successfully"
echo "Visit http://192.168.2.10:82 to proceed"