1) Add A reference to Iniect.io (or add the Factory.cs file to the project)
2) Create a custom ControllerFactory inherriting from DefaultControllerFactory and user Factory.inject(controller)
3) Hookup the new ControllerFactory
4) Use the default settings or custom Mappings