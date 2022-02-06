
## TODO:
- Search props by type
  - Search props by value (to search by value I need to specify type? No If I will interpret text)
- search props by name regex

text interpretation. create search queries from text
- Harder to look for things like colors? (can look with hex)
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
