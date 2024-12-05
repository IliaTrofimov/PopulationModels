#!/bin/bash

appName="PopulationModels"
appFolder="$appName.app"

echo "Creating Mac OS application $appName in $appFolder"
echo ""

echo "Creating application structure at $appFolder ..."
mkdir -p "$appFolder/Contents"
mkdir -p "$appFolder/Contents/_CodeSignature"
mkdir -p "$appFolder/Contents/MacOS"
mkdir -p "$appFolder/Contents/Resources"

echo "Creating Info.plist and embedded.provisionprofile at $appFolder ..."
cp "./Info.plist" "$appFolder/Contents/"
touch "$appFolder/Contents/embedded.provisionprofile"

echo "Copying binaries to app folder at $appFolder ..."
cp "./PopulationModels.UI" "$appFolder/Contents/MacOS"
cp "./libAvaloniaNative.dylib" "$appFolder/Contents/MacOS"
cp "./libHarfBuzzSharp.dylib" "$appFolder/Contents/MacOS"
cp "./libSkiaSharp.dylib" "$appFolder/Contents/MacOS"

echo ""
echo "All done"
