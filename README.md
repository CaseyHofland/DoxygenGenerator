# DoxygenGenerator

## Nani is this?

A quick way to generate API documentation from inside Unity using [doxygen](https://www.doxygen.nl/index.html), with [doxygen-awesome](https://github.com/jothepro/doxygen-awesome-css) styling. Useful for generating API of your custom packages. 

## How to use?

Watch [this tutorial](https://youtu.be/ltJgXJjS_YQ) to get started.

Note that this is a personal utility tool that Dencho forked and stole.

Dencho also added in some options to the Doxygen Window that helps to modify the Doxyfile.

Tested with Doxygen 1.10

The default Doxyfile config file needs these in your PATH to work.

- Graphviz from https://graphviz.org/download/
- Doxygen from https://www.doxygen.nl/download.html

Defualt Images folder should be created manually relative to your Output path in the plugin, example:
"OUTPUT/img"
Where OUTPUT = the folder you set for the generation output


Special thanks to [JacobPennock](https://forum.unity.com/members/jacobpennock.53178/) for the original implementation and [Brogrammar](https://forum.unity.com/members/brogrammar.1045220/) for an updated version that works with more modern versions of Unity.