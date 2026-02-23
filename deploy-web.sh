#!/bin/bash

set -e  # 只要有错误就立即停止

BUILD_DIR="BuildWeb"
DEPLOY_BRANCH="gh-pages"
MAIN_BRANCH="main"

echo "🚀 Starting WebGL deployment..."

# 1️⃣ 检查当前分支
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
if [ "$CURRENT_BRANCH" != "$MAIN_BRANCH" ]; then
  echo "❌ You must run this script from '$MAIN_BRANCH' branch."
  exit 1
fi

# 2️⃣ 检查 build 是否存在
if [ ! -d "$BUILD_DIR" ]; then
  echo "❌ $BUILD_DIR not found! Build WebGL first."
  exit 1
fi

# 3️⃣ 保存当前 main commit hash
MAIN_COMMIT=$(git rev-parse HEAD)

echo "🔄 Switching to $DEPLOY_BRANCH..."
git checkout $DEPLOY_BRANCH

# 4️⃣ 确认真的在 gh-pages
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
if [ "$CURRENT_BRANCH" != "$DEPLOY_BRANCH" ]; then
  echo "❌ Failed to switch to $DEPLOY_BRANCH."
  exit 1
fi

# 5️⃣ 清空 gh-pages 内容
echo "🧹 Cleaning old deployment..."
git rm -rf . > /dev/null 2>&1 || true

# 6️⃣ 复制新 build
echo "📦 Copying new build..."
cp -r $BUILD_DIR/* .

# 7️⃣ 提交并 push
git add .
git commit -m "Deploy WebGL build from $MAIN_COMMIT" || echo "No changes to commit."
git push origin $DEPLOY_BRANCH

# 8️⃣ 切回 main
echo "↩️ Returning to $MAIN_BRANCH..."
git checkout $MAIN_BRANCH

echo "✅ Deployment complete."