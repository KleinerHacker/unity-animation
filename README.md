# unity-animation
Extended Animation Framework for Unity based on Coroutines

# install
Use this repository as GitHub URL. It contains a package manifest file.

# usage
There is a builder to create and run animations. The builder supports a lot of animation types. 
See the following code to understood how it works:

```c#
AnimationBuilder.Create(this)
    .Wait(1f) //Wait for one second
    .Animate(myCurve, mySpeed, v => {...}) //Animate along a curve only between 0 and 1!
    .AnimateConstant(1f, v => {...}) //Animate without curve constant (linear)
    .RunAll(1f, new Action[] {...}) //Run all actions with given delay of one second
    .RunRepeated(1f, 10, i => {...}) //Run repeated 10 times with delay of one second this action
    .Parallel(builder => {...}) //Run in parallel and gets a builder to create animation. This is experimental!
    .WithFinisher(() => {...}) //Execute an action after step before has finished.
    .Start();
```

Additional you can add a finisher to each step as last parameter. 

To get a better control for the animations the method `Start()` returns an `AnimationRunner` object:

```c#
var runner = AnimationBuilder.Create(this)
    ...
    .Start();
    
...

runner.Stop(); //Stop the animation
```