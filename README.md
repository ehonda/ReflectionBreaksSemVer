# Overview

This is a small proof of concept that [semantic versioning](https://semver.org/) can be "broken" (argueably not really - See [What does this mean?](what-does-this-mean)) in C# by using reflection.

# How it works

Operating with two different versions of a dependency (where one should be backwards compatible to the other) we are going to break our code using reflection.

## Dependency version 1.0.0

First, we need to check out branch `using-v1.0.0` and run it:

```bash
cd ReflectionBreaksSemVer
git checkout using-v1.0.0
dotnet run
```

We will get the following output:

```bash
SomeLib.ClassA

Method1
GetType
ToString
Equals
GetHashCode

-----------------------

Foo: True
Bar: True
```

As we can see, our dependency `SomeLib` (which here isn't actually versioned, but it could just as well come from semver-versioned nuget) exports `SomeLib.ClassA` which has a bunch of methods.

We also get to see the boolean output of two functions, `Foo` and `Bar`. For both functions we get the value `True`.

## Dependency version 1.1.0

Now we check out branch `using-v1.1.0` and run it:

```bash
git checkout using-v1.1.0
dotnet run
```

We will get the following output:

```bash
SomeLib.ClassA

Method1
Method2
GetType
ToString
Equals
GetHashCode

-----------------------
SomeLib.ClassB

GetType
ToString
Equals
GetHashCode

-----------------------

Foo: False
Bar: False
```

As we can see (and guess from the semantic version number) our dependency has added some new features. We got a new method, `Method2` on `ClassA` as well as a new exported type, `ClassB`. Since these changes add new functionality but don't alter our existing API, the dependency got only a minor bump, from `1.0.0` to `1.1.0`.

What we can also see is that now `Foo` and `Bar` output `False`.

## What's happening?

Adding new features broke our code, because we baked in assumptions about the number of exported types from `SomeLib` as well as the number of methods on `ClassA`:

```csharp
// Broken by an additional type exported from SomeLib
static bool Foo()
    => Assembly
        .LoadFrom("SomeLib.dll")
        .ExportedTypes
        .Count() == 1;

// Broken by an additional method provided by ClassA
static bool Bar()
    => Assembly
        .LoadFrom("SomeLib.dll")
        .ExportedTypes
        .Single(type => type.Name == nameof(ClassA))
        .GetMethods()
        .Length == 5;
```

# What does this mean?

Probably nothing, but there's some things we can learn from this:

- Semantic versioning is about versioning a public API. __The number of exported types in the API can't be anything we guarantee to the user, otherwise we could never add features in a non-breaking way.__
- Relying on the number of types or methods in an assembly / class probably doesn't have a lot of use cases. Could this manifest in code that uses reflection for sane stuff though?