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

            self.header = file.readline().strip().split(seperator)
            self.content = []

            for row in file:

                row_values = row.strip().split(seperator)
                if types:
                    for i in range(len(row_values)):
                        row_values[i] = types[i](row_values[i])

                self.content.append(row_values)

    def get_row(self, index):
        return self.content[index]
    
    def get_column_by_index(self, index):
        return [row[index] for row in self.content]

    def get_column_by_heading(self, header):
        
        index = self.header.index(header)
        
        return self.get_column_by_index(index)

def main():
    
    reader = DataLogReader("data/data.txt", types=[int, float, float, float])

    for row in reader.content:
        print(row)

if __name__ == "__main__":
    main()