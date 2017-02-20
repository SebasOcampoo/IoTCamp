This repo contains the docs and the hands on lab (HOL) of the IoT Camp.

The goal of this HOL is to create a web based application able to receive data and to send command to STMicroelectronics STM32 devices in an IoT scenario.   

First of all you need to connect your STM32 to the cloud. Follow the [STMicroelectronics' official guide](./Docs/stm32cube-stm32nucleo-c.md).

Then you need to add the code for the device automatic registration, you can find it [here](./Utilities/STM32RegistrationAzure.zip).

The backend architecture of the application consist of some Azure services that communicate each other.   
One of the main component of the application is a website written in ASP.NET that shows data coming from sensors in a graphical way.   
Depending by the fact that you are interested or not to the details of that application, you can decide to follow one of these two way to complete the HOL:

1. [writing code](#writing-code)
    * clone this repo
    * complete the code, create the cloud infrastructure and configure your projects following [this](/InitialProjects) instructions    
    * Run your project!

2. [without writing code](#without-writing-code)    
    * clone this repo
    * Follow [this](/FinalProjects) instructions to create the cloud infrastructure and configure the projects
    * Run your project!
    
<br>


Requirements for the hand on lab:

* [Visual Studio Community 2015](https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409)
* [Microsoft Azure SDK for .NET](https://go.microsoft.com/fwlink/?LinkId=518003&clcid=0x410)
* An active Azure subscription

    
<br>


Folders:

* [Docs](/Docs): documentation and description
* [InitialProjects](/InitialProjects): contains the website code with some missing parts. Refer to ### Writing code
* [FinalProjects](/FinalProjects): contains the complete website code. Refer to ### Without writing code
* [Utilities](/Utilities): script and other

    
<br>


NOTE: Use the following commad to avoid committing config file:

```
git update-index --assume-unchanged InitialProjects/*/*.config
git update-index --assume-unchanged FinalProjects/*/*.config
```
