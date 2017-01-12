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

const updateOrder = (order, urlSuffix, newState) => {
    return dispatch => {
        const url = config.API_URL + '/' + urlSuffix;

        fetch(url, createPostRequest({ orderId: order.id, processId: order.processId }))
            .then(response => dispatch({
                type: actionTypes.UPDATE_STATE,
                order,
                newState
            }));
    };
}

export const cancelOrder = (order) => {
    return updateOrder(order, 'cancel', 'Cancelled')
}
export const shipOrder = (order) => {
    return updateOrder(order, 'ship', 'Shipped')
}
export const deliverOrder = (order) => {
    return updateOrder(order, 'deliver', 'Delivered')
}