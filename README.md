Marduk is an update manager for Unity.  It was built to collect update() magic method calls into one place, in situations when script instance counts are very high.

Updates done through Marduk are currently about 1/3 the magic method cost, as measured in Unity profiler.

Marduk also supports amortized updates, where minimum and maximum delays are provided.  In many cases, the update() functions you're looking to batch up probably don't need to run every frame, and Marduk provides an easy way to amortize that cost over time.

