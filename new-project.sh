BASEDIR=$(dirname "$0")
cd $BASEDIR/..
echo project name: 
read NAME
echo forking project template to $BASEDIR/$NAME
git clone git@github-DOWNBEAT:Downbeat-Interactive/mobile-project-template.git $NAME
cd $NAME
git remote set-url origin git@github-DOWNBEAT:Downbeat-Interactive/$NAME.git
git remote add upstream git@github-DOWNBEAT:Downbeat-Interactive/mobile-project-template.git
read -r OAUTH < OAUTH.txt 
curl -H "Authorization: token $OAUTH" --data '{"name":"'$NAME'"}' https://api.github.com/user/repos
git push -u origin main
git push --all
cd $NAME
