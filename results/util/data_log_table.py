
class DataLogTable:
    """
    This class complements the C# DataLogTable class. The C# version writes a new entry to the datatable. 
    This version reads the table to find the corresponding data file.
    """

    def __init__(self, data_table_path="data/datatable.txt"):
        
        self.data_table_path = data_table_path

        # Get the path to all the data
        # Assumes that the datatable file is in the same directory as the data
        self.data_path = "/".join(data_table_path.split("/")[:-1])

    def get_ID(self, evolution_parameters):

        evolution_parameters_string = str(evolution_parameters)
        
        with open(self.data_table_path) as f:
            for line in f:
                if evolution_parameters_string in line:
                    return int(line.split(",")[0])

        return None

    def get_file_path(self, evolution_parameters):
        ID = self.get_ID(evolution_parameters)

        if ID is None:
            return None
            
        return f"{self.data_path}/data - {ID}.txt"
