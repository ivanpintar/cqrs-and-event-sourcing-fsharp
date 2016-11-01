import { connect } from 'react-redux';
import { addProduct, setProductQuantity, filterProducts } from '../actions';
import ProductList from '../components/ProductList';

const sortProducts = (a,b) => {
    if (a.name < b.name) return -1; 
    if (a.name > b.name) return 1;
    return 0; 
}

const getFilteredProducts = (products, filter) => {
    return products
        .filter(p => p.name.indexOf(filter) >= 0)
        .sort(sortProducts);    
}

const mapStateToProps = (state) => {
    return {
        products: getFilteredProducts(state.products, state.filter)
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        addProduct: (name, price, quantity) => {
            dispatch(addProduct(name, price, quantity));
        },
        setQuantity: (id, diff) => {
            dispatch(setProductQuantity(id, diff));
        },
        filterProducts: (text) => {
            dispatch(filterProducts(text));
        }
    };
}

const EditableProductList = connect(mapStateToProps, mapDispatchToProps)(ProductList);

export default EditableProductList;