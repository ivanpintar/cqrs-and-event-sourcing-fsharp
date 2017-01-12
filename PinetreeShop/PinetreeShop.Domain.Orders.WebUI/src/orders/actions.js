import fetch from 'isomorphic-fetch';
import config from '../config';

const createRequest = (method, body) => ({
    method,
    body,
    headers: {
        'Content-Type': 'application/json'
    }
})

const createPostRequest = (body) => createRequest('POST', JSON.stringify(body));
const createGetRequest = () => createRequest('GET');

export const actionTypes = {
    ADD_OR_UPDATE_ORDER: 'ADD_OR_UPDATE_ORDER',
    UPDATE_STATE: 'UPDATE_STATE'
}

export const getOrders = () => {
    return dispatch => {
        const url = config.API_URL + '/list';

        fetch(url, createGetRequest(url))
            .then(response => response.json())
            .then(orders => {
                orders.forEach(order => dispatch({
                    type: actionTypes.ADD_OR_UPDATE_ORDER,
                    order
                }));
            });
    }
}

const updateOrder = (orderId, processId, urlSuffix, newState) => {
    return dispatch => {
        const url = config.API_URL + '/' + urlSuffix;

        fetch(url, createPostRequest({ orderId, processId }))
            .then(response => dispatch({
                type: actionTypes.UPDATE_STATE,
                orderId,
                newState
            }));
    };
}

export const cancelOrder = (orderId, processId) => {
    return updateOrder(orderId, processId, 'cancel', 'Cancelled')
}
export const shipOrder = (orderId, processId) => {
    return updateOrder(orderId, processId, 'ship', 'Shipped')
}
export const deliverOrder = (orderId, processId) => {
    return updateOrder(orderId, processId, 'deliver', 'Delivered')
}