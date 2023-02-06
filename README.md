# Mixed Reality Knowledge Graph Visualizer

Mixed Reality Knowledge Graph Visualizer (MR-KGV) is an educational tool designed for visualizing knowledge graphs. It provides the ability to load Turtle files from a specified app folder and visualize the contained knowledge graph. Inside the app, users can place the graph in the mixed reality environment and interact with the elements of the visualized graph. The key feature of this app is that it provides the ability for the user to visualize the knowledge graph elements in different representations, namely as spheres, images, or 3D models. To use the image or 3D model representation, the user has to specify a valid URL to load the image or 3D model from the web. The user can also annotate the KG elements.

# Requirements

* Unity version 2021.3.6f1 (install through Unity Hub)
* [NuGet for Unity] (https://github.com/GlitchEnzo/NuGetForUnity)
* [i5 Toolkit for Unity] (https://github.com/rwth-acis/i5-Toolkit-for-Unity)
* ARFoundation (install through Package Manager)
* dotNetRDF (see below)

# Build

To build the project, dotNetRDF is required. You can install dotNetRDF and get it working with the project by following these steps:

1. Install `NuGet for Unity`
1. After you installed it, use it within the Unity Editor to install dotNetRDF.
1. After installing dotNetRDF, an error will occur, saying that there are multiple pre-compiled assemblies with the same name. To fix the issue, navigate to the folder where dotNetRDF is installed within the `Assets` folder. In the same folder, there will also be a folder where a version of Newtonsoft.Json is installed. Navigate to this folder and go into the `\lib\netstandard2.0` subfolder. There, rename every file named `Newtonsoft.Json` into `Newtonsoft.Json2`.
1. After you renamed the files, restart Unity.
1. dotNetRDF should be working now without errors.

# Remarks

* The Turtle files need to be placed in the `\rdf` folder within the app directory and need to be named `graph1.ttl` or `graph2.ttl`. The `\rdf` folder will be automatically created when the app is opened for the first time.