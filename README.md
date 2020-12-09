# SmarterDoctors

This is a mod for the game RimWorld by Ludeon Studios.

It helps many more Jobs are done in a smart priority sorted order, instead of just randomly.

# Table of Contents

* [Introduction](#introduction)
* [Explanation](#explanation)
* [How to Install](#how-to-install)
* [How to Update](#how-to-update)
* [Bugs, New Features, and Updates](#bugs-new-features-and-updates)

# Introduction

You'll add the mod. You'll enable the mod.

There is no GUI, and no options. It just works.

It helps many more Jobs are done in a smart priority sorted order, instead of just randomly.

# Explanation

As the inspiring primary feature, Doctors now treat patients in order of Fatal Disease Delta (Immunity - Severity), and then bleeding patients in order of time to bleed out.

Patients in bed are now fed in order of Malnutrition severity.

Animal Training is done in priority order of falling Tameness, so you don't lose any animals. Then, it is done in reverse order, where animals with more accumulated training already are trained more, and then animals with lesser training are done later. This means, if you specify too many Labradors to be trained in Hauling, instead of seeing all of them trained only halfway, you'll see some trained to full, and some not trained at all. Trainers won't waste time on more animals than they can handle.

For Sowing and Harvesting, higher fertility soil is done before lower priority soil. All things being equals, Haygrass is done last.

For Plant Cutting, all things being equal, Haygrass is done last.

For Construction, floors are a lower priority, and the spike trap is an even lower priority (it takes a disproportionate amount of work, especially if you have lots of them, and you don't want it to distract from building coolers/heaters/beds).

For Rescue to Bed, colonists are rescued before animals.

# How to Install

At the top of this page, on the right-hand-side, a little ways down, will be a green button, labeled "Clone or download". Click it, then click "Download ZIP". Your browser will download it.

Unzip it, and it will spew out a single folder, which is probably named something like `SmarterDoctors-master`.

Assuming you are working with default installation directories on a Windows system, you will want to move this entire folder into:

`C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods`

If you did it correctly, the result should be a directory structure that looks something like this:

`C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\SmarterDoctors-master\Assemblies`

Then restart RimWorld and enable it like any other mod.

# How to Update

First and foremost, please note that I never test updating mods on older saved games. You can try it, but please assume that a new game might always be necessary.

I also don't explicitly test whether the mod can be disabled on an existing game. Please also assume that a new game might always be necessary.

With that out of the way:

Updating is just deleting the previous version of the mod and then installing a new version.

So again, assuming default installation directories on a Windows system, you'll want to delete the same folder that you added during installation, which probably looks something like:

`C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\SmarterDoctors-master`

Then follow the previous instructions to download and install the new version, by repeating the same steps as installing the original version.

# Bugs, New Features, and Updates

You are currently looking at a GitHub repository for managing application code. I work out of this GitHub repository, and so to talk about bugs, new features, or updates, you need to know a little bit about navigating a GitHub repository like this one.

Beneath the aforementioned green button "Clone or download", it will say "Latest commit", followed by a couple random characters, followed by an amount of time. This stamp indicates how long ago the mod was last updated.

So if you think you found a problem, check this stamp. Perhaps the mod has already been updated since you downloaded it last, and you should download a new version and update. See the above instructions for how to update.

By default, you are probably looking at the "master" branch. You can see this at the top of the page, on the left-hand-side, a little ways down, it will say "Branch: something", probably "Branch: master", with a little down arrow.

The "master" branch contains the current version of the mod which I consider to be tested and stable. Mostly. I guess.

Most (but not all) of my mods have a "beta" branch for pre-release, which might offer new features or bug fixes that should probably work, theoretically, but I haven't really done much testing on, so I'm not quite sure.

So if you tried updating from the "master" branch, and you still think you found a bug or a problem, or if you just want to try the shiny new features before everybody else, consider downloading the "beta" branch and installing that instead.

To do this, just click the button where it says "Branch: master", and then click the option for "beta". Congratulations! You've changed branches! Follow the same steps to download and install, except instead of `SmarterDoctors-master` it will now be `SmarterDoctors-beta`. You can have both versions installed, but please don't try to have both versions enabled at once using the in-game Mod menu.

You will probably see other choices besides "master" and "beta", but I don't recommend clicking them. I am probably in the middle of working on them, and they are probably only halfway done, and broken, otherwise they'd already be part of "beta".

So if you tried updating the master branch, and you tried the beta branch, and you still thing you found a bug, or a problem, or want to suggest a new feature, wander over to the "Issues" tab. You can find this at the very top of the page, you are currently on the first tab "Code", you want to change to the second tab "Issues".

You can look here to see if your bug, issue, or suggestion is already present, and add comments if you wish.

If it's not there, look to the right-hand-side, click the green button "New issue", just type a Title, and Leave a comment, and then look below and click the green button "Submit new issue". I will get back to you. Maybe. Eventually. Meanwhile, other users might be able to chime in and help you too!
