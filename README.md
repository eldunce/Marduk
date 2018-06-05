OVERVIEW
========

Marduk is an update manager for Unity.  It was built to collect update() magic method calls into one place, in situations when script instance counts are very high.

Updates done through Marduk are currently about 1/3 the magic method cost, as measured in Unity profiler.

Marduk also supports amortized updates, where minimum and maximum delays are provided.  In many cases, the update() functions you're looking to batch up probably don't need to run every frame, and Marduk provides an easy way to amortize that cost over time.

USAGE
=====

For scripts that use the UpdateManager:
1. derive from `Marduk.IUpdatable`
2. Replace the `Update()` function with `ManagedUpdate`
3. Call `Marduk.UpdateManager.RegisterUpdatable()` from your object's `OnEnable` function
4. Call `Marduk.UpdateManager.DeregisterUpdatable()` from your object's `OnDisable` function

AUTHOR
======

Contact Matt Smith at @ffs_matt on twitter.

