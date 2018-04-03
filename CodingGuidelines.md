# Coding Guidelines
## Project: innovativeproject-fantasy-battle-game
Executives:
* Kur, Wojciech
* Korniewicz, Sebastian
* Ormaniec, Wojciech
* Seremak, Łukasz
* Okrasa, Patryk

## Intro
All rules written **here**, have to be respected, to allow the review to be
finished positively. In case of violation, of any rules written here, you can
be requested to change the code adequately, and even **REVERTED**.

## Include statements
Every file should start with include section:

```C#
using UnityEngine;
using ANY_OTHER_PACKAGE;

// Down here your code
```

## Namespaces and file naming convention

Every script file which has been created should be named, as _Namespace_
it contains, and the namespace should be named with *PascalCase* rules.

>Active file -> Map

```C#
using UnityEngine;

namespace Map
{
  // Code which you want to put, goes inside
}
```

## Whitespaces

Every line should **never** end with whitespace. There are two main situations
in which you can find commonly:

```C#
void someFunction(int x, int y)
{
  someFunction(
    x,y
    );

    someFunction(x,y);
}
```

in this case, you can notice TAB alignment on break, and space after `(` or `x,y`
call. It is best to remove any other white spaces than `\n`.

## Braces
We shall use standard convention, which is both braces on new line. Which is:
```C#
void func()
{
	if(false)
	{

	}
	else
	{

	}

	class name
	{

	}
}
```

with some exceptions. We will allow for one line definitions, `if` statements
and initializers (props).

```C#
public void prop{get;set;}
public int func()
{
	if(true) {return 1;}
	else {return 2;}
}
```

## Class/Method/Struct naming conventions

It may appear strange, but class and method got a lot in common. In our project
we shall follow same naming conventions for all of them.

### Private & Protected
Starts with small letters
```C#
private class className{}
private funtionName(){}
private struct structName{}
```

### Internal
Due to hack'y nature, it require special treatment. Add `_` at the end of the
function, so everyone sees it.
```C#
private class className_{}
private funtionName_(){}
private struct structName_{}
```

### Public

```C#
private class ClassName{}
private FuntionName(){}
private struct StructName{}
```

## Properties naming convention
Properties are tricky. You define type of the prop, by the most available
keyword. So if you have `public prop{get;private set;}` it is a public prop.
### Private
Starts with small letters and follow with PascalCase and ends with `_`.
```C#
private int prop_{get;set;}
```

### Internal & Protected
Starts with small letters and follow with PascalCase.
```C#
protected int prop{get;set;}
```

### Public

```C#
private int Prop{get;set;}
```

## Field naming convention

### Private
Starts with small letters and ends with `_`
```C#
private int field_;
protected int prop_;
```

### Internal & protected
Starts with small letters and follow with PascalCase.
```C#
private int field;
protected int prop;
```
