import matplotlib.pyplot as plt

from data_log_reader import DataLogReader

class DataVisualiser:

    def __init__(self, path):

        reader = DataLogReader(path, types=[int, float, float, float])

        self.nruns = reader.nruns

        self.generation_values = reader.get_column_by_heading("Generation")
        self.best_values = reader.get_column_by_heading("Best")
        self.avg_values = reader.get_column_by_heading("Avg")
        self.worst_values = reader.get_column_by_heading("Worst")

    def plot_all_runs(self):
        
        for i in range(self.nruns):

            alpha = max(1/self.nruns, 0.5)
            plt.plot(self.generation_values[i], self.best_values[i], label="Best", color="C0", alpha=alpha)
            plt.plot(self.generation_values[i], self.avg_values[i], label="Avg", color="C1", alpha=alpha)
            plt.plot(self.generation_values[i], self.worst_values[i], label="Worst", color="C2", alpha=alpha)

        plt.title("Agent improvement")
        
        plt.xlabel("Generation")
        plt.ylabel("Fitness")

        # Remove duplicate labels
        handles, labels = plt.gca().get_legend_handles_labels()
        by_label = dict(zip(labels, handles))
        plt.legend(by_label.values(), by_label.keys())

        plt.show()

def main():

    data_visualizer = DataVisualiser("data/data.txt")
    data_visualizer.plot_all_runs()


if __name__ == "__main__":
    main()