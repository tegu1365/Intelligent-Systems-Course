import pandas as pd
from sklearn.model_selection import StratifiedKFold
#from sklearn.naive_bayes import MultinomialNB
from sklearn.metrics import accuracy_score
from sklearn.preprocessing import LabelEncoder
import numpy as np

# Зареждане на данните
url = "http://archive.ics.uci.edu/ml/machine-learning-databases/voting-records/house-votes-84.data"
names = ['Class Name'] + [f'V{i}' for i in range(1, 17)]
data = pd.read_csv(url, names=names)

# Предварителна обработка на данните
labels = LabelEncoder()
data['Class Name'] = labels.fit_transform(data['Class Name'])
data.replace('?', np.nan, inplace=True)
data.fillna(data.mode().iloc[0], inplace=True)
data.replace('y', 1, inplace=True)
data.replace('n', 0, inplace=True)

# Разделяне на атрибутите и класовете
x = data.drop('Class Name', axis=1)
y = data['Class Name']

# Имплементиране на Наивен Бейсов Класификатор
class NaiveBayesClassifier:
    def __init__(self, alpha=1.0):
        self.alpha = alpha
        self.class_probs = None
        self.feature_probs = None

    def fit(self, x, y):
        num_instances, num_features = x.shape
        self.classes, counts = np.unique(y, return_counts=True)

        # Изчисляване на вероятностите за класовете
        self.class_probs = np.log((counts + self.alpha) / (num_instances + len(self.classes) * self.alpha))

        # Изчисляване на вероятностите за признаците
        self.feature_probs = np.zeros((len(self.classes), num_features, 2))

        for i, c in enumerate(self.classes):
            class_instances = x[y == c]
            self.feature_probs[i, :, 0] = (class_instances == 0).sum(axis=0)
            self.feature_probs[i, :, 1] = (class_instances == 1).sum(axis=0)

        self.feature_probs = np.log((self.feature_probs + self.alpha) / (counts[:, np.newaxis, np.newaxis] + 2 * self.alpha))


    def predict_instance(self, instance):
        probs = np.zeros(len(self.classes))

        for i, c in enumerate(self.classes):
            probs[i] = self.class_probs[i]

            for j in range(len(instance)):
                probs[i] += self.feature_probs[i, j, int(instance[j])]

        return np.argmax(probs)


    def predict(self, x):
        return np.array([self.predict_instance(instance) for instance in x])

# 10-fold крос-валидация
k_fold = StratifiedKFold(n_splits=10, shuffle=True, random_state=42)
accuracies = []

for train_index, test_index in k_fold.split(x, y):
    x_train, x_test = x.iloc[train_index], x.iloc[test_index]
    y_train, y_test = y.iloc[train_index], y.iloc[test_index]

    # Инициализация и обучение на Наивния Бейсов Класификатор
    nb_classifier = NaiveBayesClassifier(alpha=1.0)
    nb_classifier.fit(x_train.values, y_train.values)

    # Предсказване и измерване на точността
    y_pred = nb_classifier.predict(x_test.values)
    accuracy = accuracy_score(y_test, y_pred)
    accuracies.append(accuracy)

# Извеждане на резултатите
for i, acc in enumerate(accuracies):
    print(f"Fold {i + 1}: Accuracy = {acc}")

# Средна точност
mean_accuracy = np.mean(accuracies)
print(f"\nMean Accuracy: {mean_accuracy}")
