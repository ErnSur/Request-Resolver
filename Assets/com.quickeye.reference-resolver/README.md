
## TODO:
- search props by name regex
- multiselect option for prop list. It would allow to do multi-object operations in the inspector
text interpretation. create search queries from text
- Drag n Drop support for object onto query field.
- finish component browser

## New View
similar to Athena query view
- Run button to run query on selected game objects
- Query field on the top with tabs
  - each tab has its own custom name and query (this way you can do multiple things in one window)
- Tree view with columns on the bottom (serialized field,Component,GameObject,)
  - GameObject foldout (so solution for displaying gos and adding them to the view is done is the same place)
    - properties
- Optional Component Browser on the side
- settings view
  - Toggle focus mode (focus object in scene, select in hierarchy)
  - toggle, select next prop when assigned
  - 
```
// looks for a prop with name that ends with "Controller"
.*Controller$

// looks for prop that has type of Image
t: Image

// same but value
v: 10
v: true
v: 10f
v: "some String"
v: #F6E7E7
// Looks for a object reference with a object id "-11001"
v: *-11001

```
### Component Browser
- Merge Children and Self tabs (Self/Children)
- select what properties to find
  - choose property type and value. This would allow to find and change all occurrences of certain prefab reference

## Ideas:
- Vertical Window
  - Expandable Lists
    - GameObject and prop list are compacted to only two items
    - they expand when mouse overed
    - when selection is changed list frames selected item (when we are changing selection from code)
