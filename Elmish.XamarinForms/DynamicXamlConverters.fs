namespace Elmish.XamarinForms.DynamicViews

#nowarn "67" // cast always holds

open System
open System.Reflection
open System.Diagnostics
open System.Windows.Input
open Xamarin.Forms

[<AutoOpen>]
module Converters =
    let makeCommand f =
        let ev = Event<_,_>()
        { new ICommand with
            member x.add_CanExecuteChanged h = ev.Publish.AddHandler h
            member x.remove_CanExecuteChanged h = ev.Publish.RemoveHandler h
            member x.CanExecute _ = true
            member x.Execute _ = f() }

    let makeImageSource (image: string) = ImageSource.op_Implicit image

    let makeFileImageSource (image: string) = FileImageSource.op_Implicit image

    let makeThickness (v: double) = Thickness.op_Implicit v

    let makeGridLength (v: obj) = 
       match v with 
       | :? string as s when s = "*" -> GridLength.Star
       | :? string as s when s = "auto" -> GridLength.Auto
       | :? float as f -> GridLength.op_Implicit f
       | :? GridLength as v -> v
       | _ -> failwithf "gridLength: invalid argument %O" v

    let makeFontSize (v: obj) = 
        match box v with 
        | :? string as s -> (FontSizeConverter().ConvertFromInvariantString(s) :?> double)
        | :? int as i -> double i
        | :? double as v -> v
        | _ -> System.Convert.ToDouble(v)

type CustomViewCell() = 
    inherit ViewCell()
    static let t = System.Runtime.CompilerServices.ConditionalWeakTable()

    override x.OnBindingContextChanged () =
        base.OnBindingContextChanged ()
        match x.BindingContext with
        | null -> 
            t.Remove(x) |> ignore
        | bcNew -> 
            match t.TryGetValue(x) with 
            | true, bcPrev -> 
                let ty = bcNew.GetType()
                let res = ty.InvokeMember("ApplyIncremental",(BindingFlags.InvokeMethod ||| BindingFlags.Public ||| BindingFlags.Instance),null, bcNew, [| box bcPrev; box x.View |] )
                t.Remove(x) |> ignore
                t.Add(x, bcNew)
                ignore res
            | false, _ -> 
                let ty = bcNew.GetType()
                let res = ty.InvokeMember("Create",(BindingFlags.InvokeMethod ||| BindingFlags.Public ||| BindingFlags.Instance),null, bcNew, [| |] )
                match res with 
                | :? View as v -> 
                    x.View <- v
                | _ -> 
                    failwithf "The cells of a ListView must each be some kind of 'View' and not a '%A'" (res.GetType())
                t.Add(x, bcNew)
                ignore res
                
type CustomListView() = 
    inherit ListView(ItemTemplate=DataTemplate(typeof<CustomViewCell>))

