# ScriptToUpdateMetadataSchemaInPageAndPublish
Script To Update Metadata Schema In Page and Publish

Let’s go back to the discussion of CoreService API (Powerful API for Web/Tridion Developer). 
Recently I got chance to work in a Migration project. Where I have come up with few requirements which is not very common. So planned to prepare some script and share it with everyone.
First scenario was customer wants to update the fields of the page metadata based on page template and republish those pages. For this type of scenario this script is very useful. 
Today I will describe how easily we can add or update field of the page metadata in one go and publish all the pages.
Steps: 
1)	Find all the pages based on page template.
2)	Add page metadata schema.
3)	Update the metadata.
4)	Re-Publish the page.

Here the requirement was need to read one field from existing component presentation of the page which are using “Page – Redirect” 
templates and update that value in one of the field of page metadata. That page metadata schema has redirect field with an embedded schema,
where we need to update the value of externalLink field.
