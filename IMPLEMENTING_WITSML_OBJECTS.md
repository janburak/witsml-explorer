## Tasks relating to implementing a WITSML object in WITSML Explorer
1. Bare minimum Witsml/Data class
2. Get endpoint in API Routes
3. Object View
4. Modify operation
5. Delete operation
6. Complete Witsml/Data class
7. Copy/paste operation
8. Checkable rows in view
9. Refresh
10. Show on server

## 1. Bare minimum Witsml/Data class
Classes in the Witsml/Data directory are used for (de)serialization of XML queries. A bare minimum implementetion requires inclusion of variables that will be shown in frontend, and the uids. A complete implementation is necessary only for copy operations. [PDS](https://witsml.pds.technology/docs/downloads/) shows what a complete (albeit without values) serialized XML templates look like. XML schema definitions (XSD) can be found [online](http://w3.energistics.org/schema/WITSML_v1.4.1.1_Data_Schema/witsml_v1.4.1.1_data/doc/witsml_schema_overview.html) or in the Resources/Xsd directory.

There are three attributes used to decide how variables are serialized.
1. `[XmlIgnore]` does not include the variable during serialization.
2. `[XmlAttribute]` relates to a field in the root tag: `<object attributeName="attributeValue">`
3. `[XmlElement]` relates to an element inside the root element: `<object><elementName>elementValue</elementName></object>`

When implementing a WITSML object there are multiple aspects of its corresponding XSD file that need to be taken into consideration.
1. `include` statements at the top of the file will include the contents of the referenced xsd files. This means that multiple files decide which elements end up in an object. It may be easiest to see in PDS what the resulting XML looks like.
2. `minOccurs` attribute, usually equal to 0 or 1, decides whether the element is optional. It does not matter for objects or strings as they are nullable. Primitives should be defined as optional as described further below.
3. `maxOccurs` attribute, usually equal to 1 or unbounded, decides whether the respective variable should be a List.
4. `type` attribute described below.

The type attribute decides the respective variable type. Many WITSML types map to the same type in our classes. For proper description of various types one can check typ_baseType.xsd or typ_dataType.xsd. The variable types can be determined by looking at the `restriction` element or the `base` attribute.  What follows is a list of C# types and what we map to them:
1. `string` - elements containing text are typically mapped to string variables. This also goes for UID elements. Strings are nullable so no extra work is needed to make them optional. All elements that should be string will likely have the abstractString WITSML type or string restriction if one follows the inheritance chain. WITSML enum types will also usually type to string, but check with the type definition to make sure. Example WITSML types: nameString, uidParentString.
2. primitives such as `int`, `bool`, or `double` - these will map to the respective abstract WITSML types. All primitive types should be made optional to avoid overwriting using default values during update queries. One way to do it is as follows:
```csharp
[XmlIgnore]
public primitiveType? ElementName { get; set; }
[XmlElement("elementName")]
public string ElementNameText
{
    get { return ElementName.HasValue ? XmlConvert.ToString(ElementName.Value) : null; }
    set { ElementName = string.IsNullOrEmpty(value) ? default(primitiveType?) : primitiveType.Parse(value); }
}
```
3. Measures TODO
4. Objects TODO
5. Lists TODO

## 2. Get endpoint in API Routes
An endpoint needs to be set up to allow WITSML Explorer to retrieve objects. 
1. In the WitsmlExplorer.Api/Query directory create a [Object]Queries.cs files with a GetWitsml[Object]ByWellbore(string wellUid, string wellboreUid) method. When working with objects further down in the hierarchy, the method needs to be adjusted to include more UIDs to sufficiently specify which objects we need to retrieve.
2. Create a WitsmlExplorer.Api/Models/[Object].cs file with a class that includes only the variables that will be expected in the frontend.
3. Create a new [Object]Service.cs file in WitsmlExplorer.Api/Services. Implement a Get[Object]s method that takes in the necessary UIDs. Call the query method that you implemented in the first step and call GetFromStoreAsync on the WitsmlClient. Return an object of the model you created in the second step.
4. Endpoints are defined in the WitsmlExplorer.Api/Routes.cs file. At the top of the class are service interfaces defined. These are bootstrapped during startup. Add a field for the service you created in the third step. Implement a Get[Object]s method that calls on the service and set up a Get endpoint in Routes constructor.
5. You can set up a test in Tests/WitsmlExplorer.IntegrationTests/Api/Services to check whether your implementation works.

## 3. Object view in Frontend
### 3.1 Adding a view for an object under wellbore
Based on [#1211](https://github.com/equinor/witsml-explorer/pull/1211)
1. Create the object interface in WitmlExplorer.Frontend/models
2. Add the object to the wellbore model
3. In navigationStateReducer.tsx:
   * add the object to SelectWellboreAction payload, 
   * add selected[Object]Group to NavigationState interface
   * set selected[Object]Group to null in allDeselected and EMPTY_NAVIGATION_STATE constants
4. Add select[Object]Group function and action to navigationStateReducer and NavigationType in WitmlExplorer.Frontend/contexts
5. Create an [Object]Service in WitmlExplorer.Frontend/services
6. Retrieve the object using the service in WellboreService, Routing, WellboresListView (this is a significant amount of code duplication and is likely subject to change in the future).
7. Retrieve the object in WellboreItem and add a tree item
8. Create an [Object]sListView in WitmlExplorer.Frontend/components/ContentViews (without context menu)
9. Add the [Object]sListView to WitmlExplorer.Frontend/components/ContentView.tsx
10. Adjust the crumb in WitmlExplorer.Frontend/components/Nav accordingly
11. Add the object to WellboreObjectTypesListView (mind "object" is verbatim). If object is not directly under wellbore, add it to the equivalent view.

### 3.2 Adding a view for an object under another parent object
Based on [#1222](https://github.com/equinor/witsml-explorer/pull/1222)
1. Create the object interface in WitmlExplorer.Frontend/models
2. Implement a Get[Object] function in [Parent]Service in WitmlExplorer.Frontend/services
3. Create a [Parent]View in WitmlExplorer.Frontend/components/ContentViews
4. In navigationStateReducer.tsx:
   * add selected[Parent] to NavigationState interface
   * set selected[Parent] to null in allDeselected and EMPTY_NAVIGATION_STATE constants
5. Add select[Parent] function and action to navigationStateReducer and NavigationType in WitmlExplorer.Frontend/contexts
6. Implement [Parent]Item in components/Sidebar
7. Add the [Parent]Item to WellboreItem
8. Add get[Parent]Crumb to Nav
9. Add the [Parent]View to WitmlExplorer.Frontend/components/ContentView.tsx
9. Add onSelect to [Parent]sListView

## 4. Modify operation
### 4.1 API
1. Create an Jobs/Modify[Object]Job.cs and add it to Models/JobType.cs
2. Add the object model to Models/EnitityType.cs
3. If an object on wellbore, add a Refresh[Object] class to Models/RefreshAction.cs
4. Implement the Workers/Modify[Object]Worker.cs. The pattern is quite similar across all modify workers.

### 4.2 Frontend
Based partially on [#1258](https://github.com/equinor/witsml-explorer/pull/1258)
1. Implement components/Modals/[Object]PropertiesModal.tsx
2. Implement [Object]ContextMenu.tsx and [Object]SidebarContextMenu.tsx in components/ContextMenus. Duplicated code can be extracted to [Object]ContextMenuUtils.tsx
3. Add [Object] to models/entityType.ts
4. Add Modify[Object] to services/jobService.tsx
5. If an object on wellbore: 
   * add an updateWellbore[Object]s function and action to navigationStateReducer.tsx
   * add Update[Object]sOnWellbore to contexts/modificationType.tsx
   * add refresh[Object] function to components/RefreshHandler.tsx

## 5. Delete operation
It might be good to combine this task with "8. Checkable rows" to allow for deleting multiple objects from the get-go.

### 5.1 API
1. Implement [Object]Reference.cs in Api/Jobs/Common
2. Create an Api/Jobs/Delete[Object]Job.cs and add it to Models/JobType.cs
3. Implement the Api/Workers/Delete[Object]Worker.cs. The pattern is quite similar across all modify workers.

### 5.2 Frontend
1. Implement [object]Reference.ts in Frontend/models/jobs
2. Add Delete[Object] to Frontend/services/jobService.tsx
3. Add delete functionality [Object]ContextMenu.tsx and [Object]SidebarContextMenu.tsx in components/ContextMenus. Duplicated code can be extracted to [Object]ContextMenuUtils.tsx

## 6. Complete Witsml/Data class
To be able to copy an object, its Witsml[Object] class should include every single property from the specification. This includes implementing the Witsml/Data classes of any objects further down in the hierarchy, such as Tubular containing TubularComponent containing MwdTool. Only current exception is ExtensionNameValue which is reported to be not used by any data providers. Given the possible large number of properties to include, it is beneficial to create a dummy object in XML that fills out all the properties, adding it to a test server, and then implementing a test akin to Tests/WitsmlExplorer.IntegrationTests/Witsml/GetFromStore/TubularTests.cs.

## 7. Copy/paste operation
It might be good to combine this task with "8. Checkable rows" to allow for deleting multiple objects from the get-go.

There is some confusion in the codebase regarding the naming of copy and paste. In frontend, we "copy" to put the object reference into the clipboard. In turn, the context menu item "paste" orders a "copy" job that does the work of adding a new instance to the target based on the source from clipboard.

### 7.1 API
1. Create an Api/Jobs/Copy[Object]Job.cs and add it to Models/JobType.cs
2. Add a Copy[Object] method Api/Query/[Object]Queries.cs. Make sure to replace the properties such as nameWell that will change based on the parent.
3. Implement the Api/Workers/Copy[Object]Worker.cs. The pattern is quite similar across all modify workers.

### 7.2 Frontend
1. Add Copy[Object] to Frontend/services/jobService.tsx
2. Implement copy[Object]Job.ts that:
   * verifies the properties of [Object]Reference 
   * parses a string to [Object]Reference
3. Add copy/paste functionality [Object]ContextMenu.tsx and [Object]SidebarContextMenu.tsx in components/ContextMenus. Duplicated code can be extracted to [Object]ContextMenuUtils.tsx.
4. If object on a wellbore, also add the option to paste the object in WellboreContextMenu.

## 8. Checkable rows in view
By setting the "checkableRows" flag in the ContentTable element in a view, it is possible mark multiple objects to perform an operation. Look at [#1294](https://github.com/equinor/witsml-explorer/pull/1294/) for an example of rewriting existing functionality to allow for checkable rows (much of it is changing variable names to plural).
1. Delete and Copy jobs in Api/Jobs/common need to allow for multiple objects.
2. Workers in Api/Workers must be rewritten. Objects on wellbore will likely require one query per object, like for Tubular. Lower level objects such as TubularComponent might be possible to use a single query for all objects.
3. Set the checkableRows flag in the view.
4. Make sure context menus in frontend handle multiple objects correctly.

## 9. Refresh
TODO

## 10. Show on server
TODO
