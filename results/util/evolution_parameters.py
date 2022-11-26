
class EvolutionParameters:
    """
    This class holds all adjustable parameters. It is implemented the same
    as the C# variant.
    """

    def __init__(self, population_size, selection_name, weight_init_name, bias_init_name, activation_name,
            mutation_rate, weight_mutate_name, bias_mutate_name, hidden_layers, fov, nrays):
        
        self.population_size = population_size
        self.selection_name = selection_name
        self.weight_init_name = weight_init_name
        self.bias_init_name = bias_init_name
        self.activation_name = activation_name
        self.mutation_rate = mutation_rate
        self.weight_mutate_name = weight_mutate_name
        self.bias_mutate_name = bias_mutate_name
        self.hidden_layers = hidden_layers
        self.fov = fov
        self.nrays = nrays

    def __str__(self):

        most_parameters = ",".join([str(self.population_size), self.selection_name, self.weight_init_name, self.bias_init_name, self.activation_name, str(self.mutation_rate), self.weight_mutate_name, self.bias_mutate_name])

        layers = f"[{';'.join(self.hidden_layers)}]"
        agent = f"{self.fov},{self.nrays}"

        return f"{most_parameters},{layers},{agent}"