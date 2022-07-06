import numpy as np
import tensorflow as tf
import matplotlib.pyplot as plt
from typing import Tuple
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.ensemble import AdaBoostClassifier
from sklearn.preprocessing import MinMaxScaler
import data


def split(X: np.ndarray, y: np.ndarray, train_size: float, valid_size: float, test_size: float) -> Tuple:
    if train_size + valid_size + test_size != 1.0:
        raise ValueError("Wrong proprotions.")
    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=test_size + valid_size
    )
    relative_valid_size = valid_size / (valid_size + test_size)
    X_test, X_valid, y_test, y_valid = train_test_split(
        X_test, y_test, test_size=relative_valid_size
    )
    return X_train, X_valid, X_test, y_train, y_valid, y_test


if __name__ == '__main__':

    games, results = data.get_vectors()
    X = np.array(games)
    y = np.array(results)
    # np.save('./x_values.npy', X, allow_pickle=True)
    # np.save('./y_values.npy', y, allow_pickle=True)
    # X = np.load('./x_values.npy', allow_pickle=True)
    # y = np.load('./y_values.npy', allow_pickle=True)
    X_train, X_valid, X_test, y_train, y_valid, y_test = split(
        X, y, train_size=0.8, valid_size=0.1, test_size=0.1
    )

    # model = AdaBoostClassifier(n_estimators=300)
    # model.fit(X_train, y_train)
    # print(model.score(X_test, y_test))
    #
    # train_dataset = tf.data.Dataset.from_tensor_slices((X_train, y_train))
    # valid_dataset = tf.data.Dataset.from_tensor_slices((X_valid, y_valid))
    # test_dataset = tf.data.Dataset.from_tensor_slices((X_test, y_test))
    #
    # BATCH_SIZE = 50
    # SHUFFLE_BUFFER_SIZE = 1000
    #
    # train_dataset = train_dataset.shuffle(SHUFFLE_BUFFER_SIZE).batch(BATCH_SIZE)
    # valid_dataset = valid_dataset.shuffle(SHUFFLE_BUFFER_SIZE).batch(BATCH_SIZE)
    # test_dataset = test_dataset.shuffle(SHUFFLE_BUFFER_SIZE).batch(BATCH_SIZE)
    #
    # model = tf.keras.Sequential([
    #     tf.keras.layers.Dense(512, activation='relu'),
    #     tf.keras.layers.Dense(1024, activation='relu'),
    #     tf.keras.layers.Dense(1, activation='sigmoid')
    # ])
    #
    # model.compile(optimizer=tf.keras.optimizers.SGD(momentum=0.95),
    #               loss=tf.keras.losses.BinaryCrossentropy(),
    #               metrics=['accuracy'])
    #
    # history = model.fit(
    #     X_train,
    #     y_train,
    #     epochs=30,
    #     verbose=2
    # )
    #
    # valid_acc = history.history["accuracy"]
    # train_losses = history.history["loss"]
    #
    # loss, accuracy = model.evaluate(test_dataset)
    #
    # print("Loss: ", loss)
    # print("Accuracy: ", accuracy)
    #
    # plt.figure(figsize=(10, 5))
    # plt.plot(train_losses)
    # plt.title("Training loss over epochs")
    # plt.xlabel("Epoch")
    # plt.ylabel("Loss")
    # plt.show()
    #
    # plt.figure(figsize=(10, 5))
    # plt.plot(valid_acc)
    # plt.title("Accuracy on validation set")
    # plt.xlabel("Epoch")
    # plt.ylabel("Accuracy")
    # plt.show()
