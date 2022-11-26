import matplotlib.pyplot as plt
import numpy as np

from util.data_log_reader import DataLogReader

class DataVisualiser:

    def __init__(self, path):

        reader = DataLogReader(path, types=[int, float, float, float])

        self.nruns = reader.nruns

        self.generation_values = reader.get_homogeneous_data_by_heading("Generation")
        self.best_values = reader.get_homogeneous_data_by_heading("Best")
        self.avg_values = reader.get_homogeneous_data_by_heading("Avg")
        self.worst_values = reader.get_homogeneous_data_by_heading("Worst")

    def plot_a_run(self, i, show=True, minimum_y_min=-25, **kwargs):
        plt.plot(self.generation_values[i], self.best_values[i], label="Best", color="C0", **kwargs)
        plt.plot(self.generation_values[i], self.avg_values[i], label="Avg", color="C1", **kwargs)
        plt.plot(self.generation_values[i], self.worst_values[i], label="Worst", color="C2", **kwargs)

        if show:
            self.apply_default_axis()
            plt.show()

    def plot_all_runs(self, show=True, minimum_y_min=-2000, **kwargs):
        
        for i in range(self.nruns):

            alpha = max(1/self.nruns, 0.2)
            self.plot_a_run(i, show=False, alpha=alpha, **kwargs)

        if show:
            self.apply_default_axis()
            plt.show()

    def plot_combined(self, show=True):
        
        data_to_plot = (
            ("Best", self.best_values),
            ("Avg", self.avg_values),
            ("Worst", self.worst_values),
        )

        for data in data_to_plot:
            label = data[0]
            values = data[1]

            mean, std_dev = compute_mean_and_std(values)

            generation_values = np.arange(len(mean))

            plt.plot(generation_values, mean, label=label)
            plt.fill_between(generation_values, mean - std_dev, mean + std_dev, alpha=0.2)

        if show:
            self.apply_default_axis()
            plt.show()

    def apply_default_axis(self, minimum_y_min=-25):
                
        # Very negative worst values are chopped of 
        bottom, top = plt.ylim()
        ymin = max(bottom, minimum_y_min)
        plt.ylim(bottom=ymin)

        plt.title("Agent improvement")
        
        plt.xlabel("Generation")
        plt.ylabel("Fitness")

        # Remove duplicate labels
        handles, labels = plt.gca().get_legend_handles_labels()
        by_label = dict(zip(labels, handles))
        plt.legend(by_label.values(), by_label.keys())

def compute_mean_and_std(np_array, axis=0):
    return np.mean(np_array, axis=axis), np.std(np_array, axis=axis)

def set_mpl_font_sizes():
    SMALL_SIZE = 20
    MEDIUM_SIZE = 24
    BIGGER_SIZE = 28

    plt.rc('font', size=SMALL_SIZE)          # controls default text sizes
    plt.rc('axes', titlesize=SMALL_SIZE)     # fontsize of the axes title
    plt.rc('axes', labelsize=MEDIUM_SIZE)    # fontsize of the x and y labels
    plt.rc('xtick', labelsize=SMALL_SIZE)    # fontsize of the tick labels
    plt.rc('ytick', labelsize=SMALL_SIZE)    # fontsize of the tick labels
    plt.rc('legend', fontsize=SMALL_SIZE)    # legend fontsize
    plt.rc('figure', titlesize=BIGGER_SIZE)  # fontsize of the figure title

    plt.rcParams["figure.figsize"] = (20,10)


def main():

    data_visualizer = DataVisualiser("../data/data - termination.txt")
    data_visualizer.plot_a_run(0)
    data_visualizer.plot_all_runs()
    data_visualizer.plot_combined()


if __name__ == "__main__":
    main()