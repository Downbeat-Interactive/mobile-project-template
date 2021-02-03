BASEDIR=$(dirname "$0")
cd $BASEDIR/..
echo project name: 
read NAME
echo forking project template to $BASEDIR/$NAME
git clone git@github-DOWNBEAT:Downbeat-Interactive/mobile-project-template.git $NAME
cd $NAME
git remote set-url origin git@github-DOWNBEAT:Downbeat-Interactive/$NAME.git
git remote add upstream git@github-DOWNBEAT:Downbeat-Interactive/mobile-project-template.git
curl -H "Authorization: token 7ee740375ee23e2ab9f63c5f9b68c9840a9e0cde" --data '{"name":"'$NAME'"}' https://api.github.com/user/repos
git push -u origin main
git push --all
cd $NAME
