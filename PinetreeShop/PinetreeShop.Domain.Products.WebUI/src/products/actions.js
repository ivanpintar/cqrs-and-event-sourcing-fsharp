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
    ADD_OR_UPDATE_PRODUCT: 'ADD_OR_UPDATE_PRODUCT',
    CHANGE_PRODUCT_QUANTITY: 'CHANGE_PRODUCT_QUANTITY'
}

export const getProducts = () => {
    return dispatch => {
        const url = config.API_URL + '/list';

        fetch(url, createGetRequest(url))
            .then(response => response.json())
            .then(products => {
                products.forEach(product => dispatch({
                    type: actionTypes.ADD_OR_UPDATE_PRODUCT,
                    product
                }));
            });
    }
}

export const addProduct = (name, price, quantity) => {
    return dispatch => {
        const url = config.API_URL + '/create';

        fetch(url, createPostRequest({ name, price, quantity }))
            .then(response => response.json())
            .then(product => dispatch({
                type: actionTypes.ADD_OR_UPDATE_PRODUCT,
                product
            }));
    };
}

export const changeProductQuantity = (id, quantity) => {
    return dispatch => {
        const url = config.API_URL + '/quantity';

        fetch(url, createPostRequest({ id, quantity }))
            .then(response => response.json())        
            .then(product => dispatch({
                type: actionTypes.CHANGE_PRODUCT_QUANTITY,
                id: product.id,
                quantity: product.quantity
            }));
    };
}