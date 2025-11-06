# LivEnvironmentHider

Hides environmental elements from the LIV camera so that you can use chroma key or other green screen methods in OBS or in video editing programs

Note: Hiding the environment from LIV also hides them in Rock cam

## Configuration 
Applied at scene load
### Green screen options
| Option | Default | Description|
| --- |  --- | --- |
| Green Screen Active | true | |
| Green Screen Color | #FF00FF| Hex code for color you want your green screen to be. This will be the same color you use in OBS's chroma key filter
| Hide Combat Floor | false | Hides the combat floor from LIV
| Hide Combat Ring | false | Hides the combat ring clamp from LIV |

### Keyboard inputs
| Option | Default | Description|
| --- |  --- | --- |
|Modifier Key| LeftAlt | Use with color keys to change screen color (will overwrite your default green screen color preference) |

#### Control keys
| Key | Action |
| --- |  --- | --- |
| Z | Toggle green screen on/off |
| F | Toggle hiding combat floor on/off |
| Q | Toggle hiding ring clamp on/off |

#### Color keys 
| Key | Color | Hex |
| --- |  --- | --- |
| K | Black | ![#000](https://placehold.co/15x15/000000/000000.png) `#000000` (Use with luma key filter) | 
| M | Magenta | ![#F0F](https://placehold.co/15x15/FF00FF/FF00FF.png) `#FF00FF` |
| G | Green | ![#0F0](https://placehold.co/15x15/00FF00/00FF00.png) `#00FF00` |
| B | Blue | ![#00F](https://placehold.co/15x15/0000FF/0000FF.png) `#0000FF` |


### Compatibility with other mods
| Option | Default | Description|
| --- |  --- | --- |
| Hide Environment Delay | 2.0 | Delay in seconds for hiding the map environment. Too short of a delay will make the environment visible in your opponent's rumble hud portrait


