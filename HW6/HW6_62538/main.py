import pandas as pd
import numpy as np
from sklearn.model_selection import KFold


class Node:
    def __init__(self, value=None, attribute=None, branches=None, leaf_class=None):
        self.value = value
        self.attribute = attribute
        self.branches = branches
        self.leaf_class = leaf_class


def calculate_entropy(cla):
    unique_classes, class_counts = np.unique(cla, return_counts=True)
    probabilities = class_counts / len(cla)
    entropy = -np.sum(probabilities * np.log2(probabilities))
    return entropy


def calculate_information_gain(attr, cla, attribute):
    total_entropy = calculate_entropy(cla)

    values, value_counts = np.unique(attr[attribute], return_counts=True)

    weighted_entropy = 0
    for value, count in zip(values, value_counts):
        subset_y = cla[attr[attribute] == value]
        weighted_entropy += (count / len(cla)) * calculate_entropy(subset_y)

    information_gain = total_entropy - weighted_entropy
    return information_gain


def id3(attr, cla, attributes, parent=None):
    unique_classes, class_counts = np.unique(cla, return_counts=True)

    # If all examples have the same class, return a leaf node
    if len(unique_classes) == 1:
        return Node(leaf_class=unique_classes[0])

    # If there are no attributes left to split on, return a leaf node with the majority class
    if len(attributes) == 0:
        majority_class = unique_classes[np.argmax(class_counts)]
        return Node(leaf_class=majority_class)

    # Select the attribute with the highest information gain
    best_attribute = max(attributes, key=lambda a: calculate_information_gain(attr, cla, a))

    # Create a node with the selected attribute
    node = Node(attribute=best_attribute, branches={})

    # Recursively build subtrees for each value of the selected attribute
    for value in np.unique(attr[best_attribute]):
        subset_X = attr[attr[best_attribute] == value]
        subset_y = cla[attr[best_attribute] == value]

        if len(subset_y) == 0:
            majority_class = unique_classes[np.argmax(class_counts)]
            node.branches[value] = Node(leaf_class=majority_class)
        else:
            node.branches[value] = id3(subset_X.drop(best_attribute, axis=1), subset_y, attributes - {best_attribute},
                                       node)

    return node


def predict(tree, example):
    if tree.leaf_class is not None:
        return tree.leaf_class
    else:
        value = example[tree.attribute]
        if value in tree.branches:
            return predict(tree.branches[value], example)
        else:
            # If the value is not in the tree, return the majority class
            return max(tree.branches.values(), key=lambda x: x.leaf_class).leaf_class


def k_fold_cross_validation(attr, cla, k=10):
    kf = KFold(n_splits=k, shuffle=True, random_state=42)
    accuracies = []

    for train_index, test_index in kf.split(attr):
        attr_train, attr_test = attr.iloc[train_index], attr.iloc[test_index]
        classes_train, classes_test = cla.iloc[train_index], cla.iloc[test_index]

        attributes = set(attr_train.columns)
        tree = id3(attr_train, classes_train, attributes)

        correct_predictions = sum(predict(tree, example) == true_class for _, (index, example), true_class in
                                  zip(attr_test.iterrows(), attr_test.iterrows(), classes_test))
        accuracy = correct_predictions / len(classes_test)
        accuracies.append(accuracy)

    return accuracies


# Load the dataset
url = "https://archive.ics.uci.edu/ml/machine-learning-databases/breast-cancer/breast-cancer.data"
columns = ['Class', 'Age', 'Menopause', 'Tumor Size', 'Inv Nodes', 'Node Caps', 'Deg Malig', 'Breast', 'Breast Quad',
           'Irradiate']
data = pd.read_csv(url, header=None, names=columns)

# Convert categorical data to numerical using one-hot encoding
data = pd.get_dummies(data,
                      columns=['Age', 'Menopause', 'Tumor Size', 'Inv Nodes', 'Node Caps', 'Breast', 'Breast Quad',
                               'Irradiate'])

# Convert the target variable 'Class' to numerical
data['Class'] = data['Class'].map({'no-recurrence-events': 0, 'recurrence-events': 1})

# Define the features and target variable
features = data.drop('Class', axis=1)
classes = data['Class']

# Perform k-fold cross-validation and get accuracy scores
accuracies = k_fold_cross_validation(features, classes)

# Display accuracy for each fold
for i, accuracy in enumerate(accuracies, 1):
    print(f'Fold {i}: Accuracy = {accuracy}')

# Display the arithmetic mean of accuracy scores
mean_accuracy = np.mean(accuracies)
print(f'\nMean Accuracy: {mean_accuracy}')
