#!/bin/sh

ROOT=`dirname $(readlink -f "$0")`
if [ "$1" == "NONE" ]; then
	echo ignore env: $1
    exit 0
fi

ENV=$1
echo "target env is $ENV"

if [ "$2" == "iOS" ]; then
    platform="iOS"
elif [ "$2" == "WebGL" ]; then
    platform="WebGL"
elif [ "$2" == "Android" ]; then
    platform="Android"
elif [ "$2" == "Win64" ]; then
    platform="StandaloneWindows64"
elif [ "$2" == "OSXUniversal" ]; then
    platform="StandaloneOSX"
else
    echo not support target: $2
    exit 1
fi

TARGET="ArtRes"
ModuleName="LoveRoom"
BUILD=$ROOT/AssetBundles/Output
REPO=${jenkins_workspace}/BuildAssets/assets-${ENV}
VERSION=`cat $ROOT/version.txt`

ensure_env () {
    if [ ! -d $REPO ];then
        #git clone git@codeup.aliyun.com:lerjin/utown/assets/assets-${ENV}.git $REPO
		git clone ssh://git@172.16.2.20:222/utown/assets/assets-${ENV}.git $REPO
    fi
    cd $REPO
    echo REPO $REPO
	git reset --hard master
	git pull origin master
}

upload_files () {
	target_dir=$REPO/${TARGET}/${ModuleName}/${platform}/$VERSION
	if [ ! -d $target_dir ];then
		mkdir -p $target_dir
	else
		rm -rf $target_dir
		mkdir -p $target_dir
	fi
    cp -rf $BUILD/* $target_dir
    cd $REPO
    git add .
    git commit -m 'auto push assets' 
    git push origin master
    DATA={\"env\":\"${ENV}\",\"target\":\"${TARGET}/${ModuleName}/${platform}/$VERSION\"}
    curl -X POST -H "Content-Type: application/json" http://172.16.2.3:5000/utown/assets/update -d $DATA
}


main() {
    ensure_env
    upload_files
}
main