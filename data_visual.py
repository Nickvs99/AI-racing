import matplotlib.pyplot as plt
import numpy as np

from data_log_reader import DataLogReader

class DataVisualiser:

    def __init__(self, path):

        reader = DataLogReader(path, types=[int, float, float, float])

        self.nruns = reader.nruns

        self.generation_values = reader.get_homogeneous_data_by_heading("Generation")
        self.best_values = reader.get_homogeneous_data_by_heading("Best")
        self.avg_values = reader.get_homogeneous_data_by_heading("Avg")
        self.worst_values = reader.get_homogeneous_data_by_heading("Worst")

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

    def plot_combined(self):
        
        data_to_plot = (
            ("Best", self.best_values),
            ("Avg", self.avg_values),
            ("Worst", self.worst_values),
        )

        for data in data_to_plot:
            label = data[0]
            values = data[1]

            mean = np.mean(values, axis=0)
            std_dev = np.std(values, axis=0)

            generation_values = np.arange(len(mean))

            plt.plot(generation_values, mean, label=label)
            plt.fill_between(generation_values, mean - std_dev, mean + std_dev, alpha=0.2)

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
    data_visualizer.plot_combined()


if __name__ == "__main__":
    main()