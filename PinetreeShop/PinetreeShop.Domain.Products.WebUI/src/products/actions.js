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
    SET_PRODUCT_QUANTITY: 'SET_PRODUCT_QUANTITY',
    FILTER_PRODUCTS: 'FILTER_PRODUCTS'
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

export const setProductQuantity = (id, quantity) => {
    return dispatch => {
        const url = config.API_URL + '/quantity';

        fetch(url, createPostRequest({ id, quantity }))
            .then(response => response.json())        
            .then(product => dispatch({
                type: actionTypes.SET_PRODUCT_QUANTITY,
                id: product.id,
                quantity: product.quantity
            }));
    };
}

export const filterProducts = (filterText) => {
    return {
        type: actionTypes.FILTER_PRODUCTS,
        filterText
    };
}