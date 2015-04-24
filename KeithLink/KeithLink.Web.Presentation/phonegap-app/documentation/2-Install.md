# Install

These steps are for a Mac

## Download the project from Git

Run the following command in the directory where you want the project to be located

```git clone ssh://<your_git_username>@scm.bekco.com:29418/ecommerce.git```

## iOS

1. Install Xcode from the Mac App Store
2. Install the iOS Simulator
	1. Open Xcode -> Xcode menu -> Preferences -> Downloads -> Components
	2. Select the simulator(s) you want to download
3. Install homebrew

```ruby -e "$(curl -fsSL https://raw.github.com/Homebrew/homebrew/go/install)”```

4. Install Node ```brew install node```
5. Install necessary Node packages using the following terminal commands
	1. ```sudo npm install -g grunt```
	2. ```sudo npm install -g phonegap```
	3. ```sudo npm install -g ios-sim```
	4. ```sudo npm install -g ios-deploy```
6. Install Ruby (already installed on Macs)
7. Install Sass with ```sudo gem install sass```
8. Install Ant ```brew install ant```

## Android

1. Make sure you have the [Java JDK](http://www.oracle.com/technetwork/java/javase/downloads/index.html) installed. You can check with ```javac -version```
2. Download [Android Studio](http://developer.android.com/sdk/index.html)
3. Go through Android Studio setup wizard and it will install the Android SDK
4. If you get an error about not having a JVM installed, run the following command in Terminal ```launchctl setenv STUDIO_JDK /Library/Java/JavaVirtualMachines/jdk1.8.0_25.jdk```. Found on [StackOverflow](http://stackoverflow.com/questions/27369269/android-studio-was-unable-to-find-a-valid-jvm-related-to-mac-os/27370525#27370525)
5. Add /tools and /platform-tools to your PATH
 1. Open Terminal
 2. ```cd```
 3. ```open .bash_profile```
 4. Add this to the bottom of the file ```export PATH=$PATH:/Users/mknabe/Library/Android/sdk/platform-tools:/Users/mknabe/Library/Android/sdk/tools```
 5. Save and restart Terminal
6. From the BEKPhonegap/ directory, try running ```grunt build:android:review```

> Error: Please install Android target "android-19".
Hint: Run "android" from your command-line to open the SDK manager.

Run android in Terminal which opens the Android SDK Manager
Install Android Wear Packages (it should select these automatically) and 4.4.2 (API 19)
Try running ```grunt phonegap:build:android``` again

7. ```grunt phonegap:run:android:emulator```
8. Install cordova ```npm install -g cordova```