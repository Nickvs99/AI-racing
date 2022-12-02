# AI-racing

This project aims to implement evolutionary algorithms on a physics-based car controller to produce a self-driving racing car. 

Generation 0 | Generation 50
:-: | :-:
<video src='docs/gen_0.mp4'> | <video src='docs/gen_50.mp4'/>

The car is controlled through a neural network. The neural network has as input the current speed and $n$ vision rays, which measure the distance until the edge of the track and simulate the sight of the driver. This input is propagated through the neural network by weights, biases and activation functions. Ultimately, this ends at the output layer, which sets the gas pedal, brake pedal and steering input of the car.

Testing all sets of different biases and weights is unfeasible. This is where the evolutionary algorithm comes in. The evolutionary algorithm attempts to find an optimal set of weights and biases through a systematic search in the solution space.

## Evolutionary algorithm
Evolution is the inspiration of evolutionary algorithms. In evolution, individuals with favourable properties are able to reproduce and pass on their genes to the next generation. Their genes are a set of instructions, which are expressed to determine the fitness of an individual. Mutations in the DNA can alter an individual's fitness, either positively or negatively. Beneficiary mutations increase the likelihood that an individual is able to reproduce. Thus, over many generations, this will result in an individual which is optimized to the environment.

These mechanisms are captured through several steps in evolutionary algorithms. 

 1. A population of size $n$ is created, where each agent is initialized with a random set of "DNA”.
 2. The fitness of each agent is evaluated.
 3. A _selection_ of agents is created. These agents are able to reproduce their genes. This selection process uses the fitness of the agents to determine the next generation.
 4. Mutations are applied to the DNA. 
 5. Repeat steps 2-4 until a termination condition is met. 

The DNA of the agents are the weights and biases of the neural network. Through mutations, the weights and biases are altered. The fitness of the agent is determined by the amount of distance travelled from its starting point in a fixed time interval. 

## Code structure
The code is structured in three distinct regions
 - AI-Racing, the unity project, which contains all the assets visualise and run the experiments. There are two scenes available. 
   - "Playground”, which lets you control the car by yourself.
   - "Track”, which contains the logic to perform the EA on the agent. 
 - data, contains the data and a data table, which maps an experiment ID with the parameters it ran with.
 - results, shows how different parameters impact the fitness of the best agents through jupyter notebooks. It also includes the reasons for when a population has finished evolving.

## How to run?
Different parameters can be tested through the batch runner. The batch runner is available in the "Track” scene. After clicking play, the batch runner will test all different parameter combinations set through the inspector and save the results to the data folder. 

The batch runner can be slowed down by toggling "p”. This allows you to see the current behaviour of the agent. 

## TODO
 - Generalise agent. Currently, in each generation, the agents start at the same starting point. This can results in agents only learning the start of the course and would thus not be able to drive any randomly generated track. A quick fix would be to pick a random starting point on the track. Random track generation would be even better but would require far more time.
 - Implementation of crossover. Crossover is the method of combining two or multiple sets of DNA from the parent. This should increase the diversity in neural networks and thus a broader search through the solution space.
 - Improve car-physics. The car feels slow and unresponsive to the current parameters.

