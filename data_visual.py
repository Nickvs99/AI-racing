import matplotlib.pyplot as plt

from data_log_reader import DataLogReader

def main():

    reader = DataLogReader("data/data.txt", types=[int, float, float, float])

    generation_values = reader.get_column_by_heading("Generation")
    best_values = reader.get_column_by_heading("Best")
    avg_values = reader.get_column_by_heading("Avg")
    worst_values = reader.get_column_by_heading("Worst")

    plt.plot(generation_values, best_values, label="Best")
    plt.plot(generation_values, avg_values, label="Avg")
    plt.plot(generation_values, worst_values, label="Worst")

    plt.title("Agent improvement")
    
    plt.xlabel("Generation")
    plt.ylabel("Fitness")

    plt.legend()
    plt.show()

if __name__ == "__main__":
    main()