#!/bin/bash

echo "Building Unity WebGL..."

# 假设你 build 到 BuildWeb 文件夹

BUILD_DIR="BuildWeb"

if [ ! -d "$BUILD_DIR" ]; then
  echo "BuildWeb not found!"
  exit 1
fi

echo "Switching to gh-pages..."
git checkout gh-pages

echo "Removing old files..."
git rm -rf . > /dev/null 2>&1

echo "Copying new build..."
cp -r $BUILD_DIR/* .

git add .
git commit -m "Deploy WebGL build"
git push origin gh-pages

echo "Returning to main..."
git checkout main

echo "Deployment complete."