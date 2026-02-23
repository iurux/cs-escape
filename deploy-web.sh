#!/bin/bash

set -e

BUILD_DIR="BuildWeb"
DOCS_DIR="docs"

echo "🚀 Deploying WebGL to docs..."

if [ ! -d "$BUILD_DIR" ]; then
  echo "❌ BuildWeb not found!"
  exit 1
fi

rm -rf $DOCS_DIR
mkdir $DOCS_DIR

cp -r $BUILD_DIR/* $DOCS_DIR/

git add $DOCS_DIR
git commit -m "Update WebGL build"
git push

echo "✅ Deployment complete."