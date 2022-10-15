import numpy as np
import numpy.ma as ma

class DataLogReader:

    def __init__(self, path, seperator=",", types=None):
        """
        Reads the data file produced by the C# DataLogger class.

        Arguments:
         - path (string): path to the data file
         - seperator (string): character which seperate the columns
         - types (list of type): the value types for each column of the data file
        """

        with open(path, "r") as file:
            lines = file.read().split("\n")

        # The data file is constructed in several sections, which are seperated by a seperator row
        # The seperator row is a bunch of ---------------------
        sections = self.determine_sections(lines)

        self.custom_comment = sections[0]
        self.parameter_cmomment = sections[1]
        
        data_sections = sections[2:]
        
        self.nruns = len(data_sections)
        self.header = data_sections[0][0].strip().split(seperator)
        
        self.data = []
        for data_section in data_sections:
            
            data_run = []
            for row in data_section[1:]:

                row_values = row.split(seperator)
                
                if types:
                    for i in range(len(row_values)):
                        row_values[i] = types[i](row_values[i])

                data_run.append(row_values)

            self.data.append(data_run)

        self.n_max_generations = max([len(values) for values in self.get_data_by_index(0)])
        
    def determine_sections(self, lines):

        sections = []

        seperator_row_indices = [index for index, line in enumerate(lines) if line == "-" * len(line) and len(line) > 0]

        # Pretend like their is a seperator row before and after the data file
        # This ensure that the first and last section are also captured
        section_row_indices = [-1, *seperator_row_indices, len(lines) - 1]

        for i in range(len(section_row_indices) - 1):

            start_index = section_row_indices[i] + 1
            end_index = section_row_indices[i + 1]
            
            if start_index >= end_index:
                continue

            sections.append(lines[start_index:end_index])

        return sections

    def get_run(self, index):
        return self.data[index]
    
    def get_data_by_index(self, index):
        return [[row[index] for row in data_section] for data_section in self.data]

    def get_data_by_heading(self, header):
        
        index = self.header.index(header)
        return self.get_data_by_index(index)

    def get_homogeneous_data_by_heading(self, header):
        """
        Returns the data in a homogeneous data format, e.g. all rows have the same length.
        This is done, because then numpy will work on the data. The extra data points
        needed to create a homogenous matrix are masked away.
        """

        n_rows = self.nruns
        n_cols = self.n_max_generations

        data = self.get_data_by_heading(header)

        homogeneous_data = np.empty((n_rows, n_cols))
        homogeneous_data.fill(None)

        mask = np.empty((n_rows, n_cols))
        mask.fill(1)

        for i in range(len(data)):
            for j in range(len(data[i])):
                homogeneous_data[i, j] = data[i][j]
                mask[i, j] = 0

        return ma.masked_array(homogeneous_data, mask)

def main():
    
    reader = DataLogReader("data/data.txt", types=[int, float, float, float])

    print(reader.header)
    print(reader.get_run(0))
    print(reader.get_data_by_index(1))
    print(reader.get_data_by_heading("Generation"))

    print(reader.get_homogeneous_data_by_heading("Generation"))

if __name__ == "__main__":
    main()