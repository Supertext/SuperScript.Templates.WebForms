using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using HtmlAgilityPack;
using SuperScript.Templates.Declarables;
using SuperScript.Templates.Exceptions;

namespace SuperScript.Templates.WebForms.Containers
{
	/// <summary>
	/// This control can be used to relocate and emit its contents in a common location.
	/// </summary>
	public class TemplateContainer : Container.WebForms.Container
    {
        #region Global Constants and Variables

        private const string ScriptNewLine = "\r\n";
        private const string ScriptTab = "\t";

        #endregion


		#region Properties

		/// <summary>
		/// Gets or sets the name of the template.
		/// </summary>
		public string Name { get; set; }

		#endregion


		protected override void __PreRender(object sender, EventArgs e)
		{
			// extract just the contents that we want to append
			var contents = GetContents();
			if (String.IsNullOrWhiteSpace(contents))
			{
				return;
			}


			AddTemplateDeclarations(GetContents());

			// now remove the contained template from its design-time location
			Controls.Clear();
		}


	    /// <summary>
	    /// Generates a JavaScript multi-line comment block containing a highlighted comment.
	    /// </summary>
	    /// <param name="comment">The comment which should appear highlighted inside the multi-line comment.</param>
	    /// <param name="startOnNewLine">Indicates whether a new line should be added before the start of the comment block.</param>
	    /// <returns>A string containing the specified comment inside a multi-line JavaScript comment block.</returns>
	    protected override string GenerateComment(string comment, bool startOnNewLine = true)
	    {
	        var messageLength = comment.Length + 2;
	        var padding = new StringBuilder(messageLength);
	        for (var i = 0; i < messageLength; i++)
	        {
	            padding.Append("*");
	        }

	        var startLine = string.Empty;
	        if (startOnNewLine)
	        {
	            startLine = string.Format("{0}{1}{1}{1}",
	                                      ScriptNewLine,
	                                      ScriptTab);
	        }
	        return string.Format("{0}/*{3}*/{1}{2}{2}{2}/* {4} */{1}{2}{2}{2}/*{3}*/{1}{2}{2}{2}",
	                             startLine,
	                             ScriptNewLine,
	                             ScriptTab,
	                             padding,
	                             comment);
	    }


	    /// <summary>
		/// Obtains the template contents that have been passed into this <see cref="TemplateContainer" /> control.
		/// </summary>
		/// <returns>A string containing the template markup that was declared inside the current instance of this control.</returns>
		protected override string GetContents()
		{
			using (var stringWriter = new StringWriter())
			using (var writer = new HtmlTextWriter(stringWriter))
			{
				base.Render(writer);

				writer.Flush();

				// this StringBuilder contains the text we wish to write into the InjectInto control
				var sb = stringWriter.GetStringBuilder();


				// ensure that any HTML comments are now converted to JavaScript comments
				sb = sb.Replace("<!--", "/* <!--")
				       .Replace("-->", "--> */");


				// add the original location comment
				//if (AddLocationComments)
				//{
				//	var filename = GetFileName();
				//	var openMessage = GenerateComment("Located dynamically from " + filename);
				//	var closeMessage = GenerateComment("End of script from " + filename);
				//	return openMessage + templateBuilder + closeMessage;
				//}

				return sb.ToString();
			}
		}


		protected void AddTemplateDeclarations(string allContents)
		{
			// load this into an HtmlAgility document.
			// - if a <script> tag is present then HtmlAgility will recognise this and we can request the InnerText
		    var doc = new HtmlDocument();
			doc.LoadHtml(allContents);

			var textNodeAdded = false;

			foreach (var node in doc.DocumentNode.ChildNodes.Where(node => node.NodeType == HtmlNodeType.Element))
			{
				if (node.Name.Equals("script", StringComparison.InvariantCultureIgnoreCase))
				{
					if (String.IsNullOrWhiteSpace(node.InnerHtml.Trim()))
					{
						continue;
					}

					var decl = new TemplateDeclaration();

					// use the 'id' or 'name' attribute as this template's name
					var attrTemplName = node.GetAttributeValue("id", null);
					if (attrTemplName == null)
					{
						attrTemplName = node.GetAttributeValue("name", null);
						if (attrTemplName == null)
						{
							throw new MissingTemplateInformationException("A TemplateDeclaration requires a name to be specified.");
						}
					}

					decl.Name = attrTemplName;

					// check if an EmitterKey (with greater specificity than being declared on the TemplateContainer) has been specified
					decl.EmitterKey = node.GetAttributeValue("emitterKey", EmitterKey);

					decl.Template = node.InnerHtml.Trim();

					var attrInsertAt = node.GetAttributeValue("insertAt", -1);
					if (attrInsertAt > -1)
					{
						Declarations.AddDeclaration<TemplateDeclaration>(decl, attrInsertAt);
					}

					Declarations.AddDeclaration<TemplateDeclaration>(decl, InsertAt);
				}
				else
				{
					if (textNodeAdded)
					{
						throw new DuplicateTemplateException("Multiple instances of TemplateDeclaration cannot be added inside a TemplateContainer unless they are contained within <script> tags, each with their own name property.");
					}

					if (String.IsNullOrWhiteSpace(Name))
					{
						throw new MissingTemplateInformationException("A TemplateDeclaration requires a name to be specified.");
					}

					if (String.IsNullOrWhiteSpace(EmitterKey))
					{
						throw new MissingTemplateInformationException("A TemplateDeclaration requires an emitter key to be specified.");
					}

					textNodeAdded = true;

					Declarations.AddDeclaration<TemplateDeclaration>(new TemplateDeclaration
						                                                 {
							                                                 Name = Name,
							                                                 Template = node.InnerHtml.Trim(),
							                                                 EmitterKey = EmitterKey
						                                                 }, InsertAt);
				}
			}
		}
	}
}