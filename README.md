_**IMPORTANT NOTE:**_ This project is currently in beta and the documentation is currently incomplete. Please bear with us while the documentation is being written.

####SuperScript offers a means of declaring assets in one part of a .NET web solution and have them emitted somewhere else.


When developing web solutions, assets such as JavaScript declarations or HTML templates are frequently written in a location that differs from their desired output location.

For example, all JavaScript declarations should ideally be emitted together just before the HTML document is closed. And if caching is preferred then these declarations should be in an external file with caching headers set.

This is the functionality offered by SuperScript.



##The TemplateContainer Control

This project contains one publicly-accessible class, `SuperScript.Templates.WebForms.Containers.TemplateContainer`.
This class derives from [`SuperScript.Container.WebForms.Container`](https://github.com/Supertext/SuperScript.Container.WebForms/blob/master/Container.cs)
(which itself derives from `System.Web.UI.WebControls.PlaceHolder`) and can be used in .NET _.aspx_ files for encapsulating 
an HTML template and converting this to an instance of `SuperScript.Templates.Declarables.TemplateDeclaration`.

For example

```HTML
<%@ Register TagPrefix="spx" Namespace="SuperScript.Templates.WebForms.Containers" Assembly="SuperScript.Templates.WebForms" %>
...
<spx:TemplateContainer runat="server" EmitterKey="templates" Name="demo-template">

	<li>{{:Name}}</li>
	
</spx:TemplateContainer>
```

In the above example the name of the client-side HTML template has been expressed as a property on the `TemplateContainer`
control. An alternative allows multiple template declarations.

```HTML
<%@ Register TagPrefix="spx" Namespace="SuperScript.Templates.WebForms.Containers" Assembly="SuperScript.Templates.WebForms" %>
...
<spx:TemplateContainer runat="server" EmitterKey="templates">

  <script type="text/html" id="template-1">
    <li>{{:Name}}</li>
  </script>

  <script type="text/html" id="template-2">
    <h1>{{:Name}}</h1>
  </script>
	
</spx:TemplateContainer>
```




The exposed properties are as follows.
* `AddLocationComments` [bool]

  Determines whether the emitted contents should contain comments indicating the original location when in debug-mode. The default value is `true`.

* `EmitterKey` [string]

  Indicates which instance of `IEmitter` the content should be added to. If not specified then the contents will be added to the default implementation of `IEmitter`.

* `InsertAt` [Nullable&lt;int&gt;]

  Gets or sets an index in the collection at which the contents are to be inserted.

* `Name`

  Gets or sets the name/id of the client-side template.


##Dependencies
There are a variety of SuperScript projects, some being dependent upon others.

* [`SuperScript.Common`](https://github.com/Supertext/SuperScript.Common)

  This library contains the core classes which facilitate all other SuperScript modules but which won't produce any meaningful output on its own.

* [`SuperScript.Container.WebForms`](https://github.com/Supertext/SuperScript.Container.Mvc)

  This library allows developers to easily declare these assets in ASP.NET WebForms _.aspx_ files. 

* [`SuperScript.Templates`](https://github.com/Supertext/SuperScript.Templates)

  This library contains functionality for making HTML template-specific declarations.
  

`SuperScript.Templates.WebForms` has been made available under the [MIT License](https://github.com/Supertext/SuperScript.Templates.WebForms/blob/master/LICENSE).
