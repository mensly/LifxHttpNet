# LifxHttpNet
### .Net wrapper around the LIFX cloud HTTP API:

* This library acts as a thin wrapper around the public LIFX HTTP API as described
  at http://developer.lifx.com
* All operations make use of the async Task APIs via
  [Refit](http://paulcbetts.github.io/refit/) for REST calls
* This library does not perform any caching or management of model objects other than to
  simplify accessing the API
* I hope that this library helps enthusiasts tinker with their LIFX lights, though much
  more backend code would be required for a full-featured Lifx client
* Use of this library requires a Personal Access token acquired from:
    https://cloud.lifx.com/settings
* Use of this library should only be in accordance to the terms at:
    http://developer.lifx.com/terms.html

### Supports the following operations:
* List Lights
* Toggle Power
* Set Power State
* Set Color
* Set State 
* Set States
* Breathe Effect
* Pulse Effect
* Cycle
* List Scenes
* Activate Scene
* Validate Color