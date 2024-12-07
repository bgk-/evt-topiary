## EvtVariables

EvtVariables are extended as EvtTopiVariables and used as runtime variable containers for any .topi `extern` variable.

Create one with `ContextMenu > Create > Evt > Topiary > [Type]`

Then set the `Topi Variable Name` to the name in the .topi file.

Topiary.Unity will automatically add the object to the Addressables `Topiary` group with labels `Topiary` and `Evt`.
Then each Dialogue will automatically load the EvtVariables and hook up callbacks with the Topiary VM.

## Installation
Can be installed via the Package Manager > Add Package From Git URL...

This repo has a dependency on the EvtVariable and Topiary.Unity packages which *MUST* be installed first. 
(From my understanding Unity does not allow git urls to be used as dependencies of packages)
`https://github.com/peartreegames/evt-variables.git`
`https://github.com/peartreegames/topiary-unity.git`

then the repo can be added

`https://github.com/peartreegames/evt-topiary.git`

