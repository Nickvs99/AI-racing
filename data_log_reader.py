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
        self.sections = self.determine_sections(lines)

        data_section = self.sections[-1]
        
        self.header = data_section[0].strip().split(seperator)
        
        self.data = []
        for row in data_section[1:]:

            row_values = row.split(seperator)
            
            if types:
                for i in range(len(row_values)):
                    row_values[i] = types[i](row_values[i])

            self.data.append(row_values)
    
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

    def get_row(self, index):
        return self.data[index]
    
    def get_column_by_index(self, index):
        return [row[index] for row in self.data]

    def get_column_by_heading(self, header):
        
        index = self.header.index(header)
        
        return self.get_column_by_index(index)

def main():
    
    reader = DataLogReader("data/data.txt", types=[int, float, float, float])

    print(reader.header)
    for row in reader.data:
        print(row)

if __name__ == "__main__":
    main()