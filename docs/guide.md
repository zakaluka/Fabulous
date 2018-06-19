Elmish.XamarinForms Guide
=======

* [Structure of an App](#structure-of-an-app)
* [Views](views.md)
* [Models](models.md)
* [Update, Messages and Control](update.md)
* [Styling](styling.md)
* [Multi-page applications and Navigation](navigation.md)

Structure of an App
------

Here is the typical structure for an app:
```fsharp
type Msg =
    | ...
    

/// The model from which the view is generated
type Model = 
    { ... }

/// Returns the initial state
let init() = { ... }
    
/// The funtion to update the view
let update (msg:Msg) (model:Model) = ...

/// The view function giving updated content for the page
let view (model: Model) dispatch = ...

type App () = 
    inherit Application ()

    let runner = 
        Program.mkSimple init update view
        |> Program.withConsoleTrace
        |> Program.withDynamicView
        |> Program.run
```



### The model

The model is the core data from which the whole state of the app can be resurrected.  When designing your model, ask yourself  "what is the information I would need on restart to get the app back to the same essential state". The model is generally immutable but may also contain elements such as service connections.

Generally the model grows organically as you prototype your app.

Some advantages of using an immutable model are:

* It is easy to unit test your `init`, `update` and `view` functions
* You can save/restore your model relatively easily
* It makes tracing causality usually very simple

The `init` function returns your initial model.

See [models](models.md).

### The view function

The view function computes an immutable Xaml-like description. In the above example, the choice between a label and button depends on the `model.Pressed` value.

See [views](views.md), [styling](styling.md) and [navigation](navigation.md).

### The `update` function

Each model gets an `update` function for message processing. The messages are either messages from the `view` or from external events.
If using `Program.mkProgram` your model may also return new commands to trigger asa result of processing a message.

See [update](update.md)
