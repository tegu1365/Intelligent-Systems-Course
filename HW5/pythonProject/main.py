import pandas as pd
from sklearn.model_selection import StratifiedKFold
from sklearn.metrics import accuracy_score
from sklearn.preprocessing import LabelEncoder
import numpy as np

# Load data
url = "http://archive.ics.uci.edu/ml/machine-learning-databases/voting-records/house-votes-84.data"
names = ['Class Name'] + [f'V{i}' for i in range(1, 17)]
data = pd.read_csv(url, names=names)

# Format data
labels = LabelEncoder()
data['Class Name'] = labels.fit_transform(data['Class Name'])  # republican=1
data.replace('?', np.nan, inplace=True)
data.fillna(data.mode().iloc[0], inplace=True)  # nan = n
data.replace('y', 1, inplace=True)  # y=1
data.replace('n', 0, inplace=True)  # n=0

# Split class and attributes
attributes = data.drop('Class Name', axis=1)
# print(attributes)
classes = data['Class Name']


# Naive Bayes
class NaiveBayesClassifier:
    def __init__(self, alpha=1.0):
        self.alpha = alpha
        self.class_probs = None
        self.attributes_probs = None

    def fit(self, attributes, classes):
        num_instances, num_attributes = attributes.shape
        self.classes, counts = np.unique(classes, return_counts=True)

        # P(Class)
        self.class_probs =(counts + self.alpha) / (num_instances + len(self.classes) * self.alpha)
        #print(self.class_probs)

        # P(attributes)
        self.attributes_probs = np.zeros((len(self.classes), num_attributes, 2))

        for i, c in enumerate(self.classes):
            class_instances = attributes[classes == c]
            self.attributes_probs[i, :, 0] = (class_instances == 0).sum(axis=0)
            self.attributes_probs[i, :, 1] = (class_instances == 1).sum(axis=0)

        self.attributes_probs = (self.attributes_probs + self.alpha) / (counts[:, np.newaxis, np.newaxis] + 2 * self.alpha)
        #print(self.attributes_probs)

    def predict_instance(self, instance):
        probs = np.zeros(len(self.classes))

        for i, c in enumerate(self.classes):
            probs[i] = self.class_probs[i]

            for j in range(len(instance)):
                probs[i] += self.attributes_probs[i, j, int(instance[j])]

        return np.argmax(probs)

    def predict(self, x):
        return np.array([self.predict_instance(instance) for instance in x])


# 10-fold
k_fold = StratifiedKFold(n_splits=10, shuffle=True) # , random_state=42
accuracies = []

for train_index, test_index in k_fold.split(attributes, classes):
    x_train, x_test = attributes.iloc[train_index], attributes.iloc[test_index]
    y_train, y_test = classes.iloc[train_index], classes.iloc[test_index]

    # Naive Bayes init and learning
    nb_classifier = NaiveBayesClassifier(alpha=1.0)
    nb_classifier.fit(x_train.values, y_train.values)

    # Predicting and accuracy
    y_pred = nb_classifier.predict(x_test.values)
    accuracy = accuracy_score(y_test, y_pred)
    accuracies.append(accuracy)

# Output
for i, acc in enumerate(accuracies):
    print(f"Fold {i + 1}: Accuracy = {acc}")

# Mean accuracy
mean_accuracy = np.mean(accuracies)
print(f"\nMean Accuracy: {mean_accuracy}")
