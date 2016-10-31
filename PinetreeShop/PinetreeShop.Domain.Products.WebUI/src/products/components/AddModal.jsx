import React from 'react';
import { FormGroup, ControlLabel, FormControl } from 'react-bootstrap'
import ModalDialog from '../../global/components/ModalDialog';

class AddModal extends React.Component{
    constructor(props) {
        super(props);
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handlePriceChange = this.handlePriceChange.bind(this);
        this.handleQuantityChange = this.handleQuantityChange.bind(this);
        this.submit = this.submit.bind(this);
        this.close = this.close.bind(this);
        this.state = { 
            name: '',
            price: '',
            quantity: 0,
            errors: {}
        }; 
    }

    handleNameChange(e) {
        this.setState({ name: e.target.value });
    }

    handlePriceChange(e) {
        this.setState({ price: e.target.value });
    }

    handleQuantityChange(e) {
        this.setState({ quantity: e.target.value });
    }

    close() {
        this.setState({
            name: '',
            price: '',
            quantity: 0,
            errors: {}
        });
        this.props.onClose();
    }

    submit() {    
        if(this.state.name && this.state.price >= 0 && this.state.price !== '' && this.state.quantity > 0) {
            this.props.addProduct(this.state.name, this.state.price, this.state.quantity);
            this.setState({ name: '', price: '', quantity: 0, errors:{} })
            this.props.onClose();
            return;
        }

        let errors = {};

        if(!this.state.name) errors.name = true;
        if(this.state.price < 0 || this.state.price === '') errors.price = true;
        if(this.state.quantity < 0) errors.quantity = true;

        this.setState({ errors: errors });
    }

    render() {
        let nameValState = null;
        let priceValState = null;
        let quantityValState = null;
        if(this.state.errors.name) nameValState = 'error';
        if(this.state.errors.price) priceValState = 'error';
        if(this.state.errors.quantity) quantityValState = 'error';

        return (
            <ModalDialog 
                show={this.props.show}
                onClose={this.close}
                onSave={this.submit}
                closeText='Cancel'
                title='Add Product'>
                <FormGroup validationState={nameValState}>
                    <ControlLabel>Name</ControlLabel>
                    <FormControl 
                            value={this.state.name}
                            type='text'
                            onChange={this.handleNameChange}/>
                </FormGroup>
                <FormGroup validationState={priceValState}>
                    <ControlLabel>Price</ControlLabel>
                    <FormControl 
                        value={this.state.price}
                        type='number' 
                        onChange={this.handlePriceChange}/>
                </FormGroup>
                <FormGroup validationState={quantityValState}>
                    <ControlLabel>Quantity</ControlLabel>
                    <FormControl 
                        value={this.state.quantity}
                        type='number' 
                        onChange={this.handleQuantityChange}/>
                </FormGroup>
            </ModalDialog>
        );
    }
}

export default AddModal;